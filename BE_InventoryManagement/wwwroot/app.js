const API_BASE = '/api/inventory';
const LOGIN_URL = '/api/login';
let inventoryData = [];

// === AUTH & FETCH HELPER ===
function getToken() {
    return localStorage.getItem('thw_jwt_token');
}

async function fetchWithAuth(url, options = {}) {
    const token = getToken();
    const headers = {
        'Content-Type': 'application/json',
        ...(options.headers || {})
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, { ...options, headers });

    if (response.status === 401) {
        logout();
        throw new Error('Nicht autorisiert.');
    }

    return response;
}

function setLogoutButtonVisible(visible) {
    const btn1 = document.getElementById('btn-logout');
    const btn2 = document.getElementById('button-logout');
    const displayVal = visible ? 'inline-flex' : 'none';
    if (btn1) btn1.style.display = displayVal;
    if (btn2) btn2.style.display = displayVal;
}

async function handleLogin(e) {
    e.preventDefault();
    const usernameInput = document.getElementById('login-username').value.trim();
    const passwordInput = document.getElementById('login-password').value;
    const errorEl = document.getElementById('login-error');

    errorEl.style.display = 'none';

    try {
        const res = await fetch(LOGIN_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username: usernameInput, password: passwordInput })
        });

        if (!res.ok) {
            errorEl.textContent = 'Benutzername oder Passwort falsch.';
            errorEl.style.display = 'block';
            return;
        }

        const data = await res.json();
        localStorage.setItem('thw_jwt_token', data.token);

        closeModal('modal-login');
        document.getElementById('login-password').value = '';
        setLogoutButtonVisible(true);

        loadInventory();
    } catch (err) {
        errorEl.textContent = 'Verbindungsfehler zum Server.';
        errorEl.style.display = 'block';
    }
}

function logout() {
    localStorage.removeItem('thw_jwt_token');
    inventoryData = [];
    const tbody = document.getElementById('inventory-tbody');
    if (tbody) tbody.innerHTML = '';
    setLogoutButtonVisible(false);
    openModal('modal-login');
}

// === INVENTAR LADEN & RENDERN ===
async function loadInventory() {
    if (!getToken()) {
        openModal('modal-login');
        return;
    }

    try {
        const response = await fetchWithAuth(API_BASE);
        if (!response.ok) throw new Error('Fehler beim Laden');

        inventoryData = await response.json();
        setLogoutButtonVisible(true);
        renderTable();
    } catch (err) {
        console.error(err);
        const tbody = document.getElementById('inventory-tbody');
        if (tbody) {
            tbody.innerHTML = `<tr><td colspan="10" class="text-center" style="color:red;">Fehler beim Laden der Daten vom Server.</td></tr>`;
        }
    }
}

function renderTable() {
    const tbody = document.getElementById('inventory-tbody');
    const searchInput = document.getElementById('search-input');
    if (!tbody) return;

    const searchTerm = searchInput ? searchInput.value.toLowerCase().trim() : '';

    const filtered = inventoryData.filter(item => {
        if (!searchTerm) return true;
        const mhdFormatted = formatDate(item.expirationDate);
        return (
            item.id.toString().includes(searchTerm) ||
            (item.articleNumber && item.articleNumber.toLowerCase().includes(searchTerm)) ||
            (item.productName && item.productName.toLowerCase().includes(searchTerm)) ||
            (item.companyName && item.companyName.toLowerCase().includes(searchTerm)) ||
            (item.quantity && item.quantity.toString().includes(searchTerm)) ||
            (item.price && item.price.toString().includes(searchTerm)) ||
            (item.comment && item.comment.toLowerCase().includes(searchTerm)) ||
            mhdFormatted.includes(searchTerm)
        );
    });

    filtered.sort((a, b) => new Date(a.expirationDate) - new Date(b.expirationDate));

    if (filtered.length === 0) {
        tbody.innerHTML = `<tr><td colspan="10" class="text-center">Keine Produkte gefunden.</td></tr>`;
        return;
    }

    tbody.innerHTML = filtered.map(item => {
        const daysLeft = calculateDaysLeft(item.expirationDate);
        const badgeClass = getBadgeClass(daysLeft);
        const commentText = item.comment ? escapeHtml(item.comment) : '<span style="color: #aaa;">-</span>';

        return `
            <tr>
                <td><strong>#${item.id}</strong></td>
                <td>${escapeHtml(item.articleNumber)}</td>
                <td><strong>${escapeHtml(item.productName)}</strong></td>
                <td>${escapeHtml(item.companyName)}</td>
                <td>${item.quantity}</td>
                <td>${item.price.toFixed(2)} €</td>
                <td>${formatDate(item.expirationDate)}</td>
                <td><span class="badge ${badgeClass}">${daysLeft < 0 ? 'Abgelaufen (' + Math.abs(daysLeft) + ' T.)' : daysLeft + ' Tage'}</span></td>
                <td>${commentText}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-secondary" onclick="openEditModal(${item.id})">✏️ Bearbeiten</button>
                    <button class="btn btn-sm btn-danger" onclick="openDeleteModal(${item.id}, '${escapeHtml(item.productName)}')">🗑️ Löschen</button>
                </td>
            </tr>
        `;
    }).join('');
}

// === HILFSFUNKTIONEN ===
function calculateDaysLeft(expirationDateStr) {
    const expDate = new Date(expirationDateStr);
    const today = new Date();
    expDate.setHours(0,0,0,0);
    today.setHours(0,0,0,0);

    const diffTime = expDate - today;
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}

function getBadgeClass(days) {
    if (days < 0) return 'badge-danger';
    if (days <= 14) return 'badge-warn';
    return 'badge-ok';
}

function formatDate(isoString) {
    if (!isoString) return '';
    const d = new Date(isoString);
    return isNaN(d.getTime()) ? isoString : d.toLocaleDateString('de-DE');
}

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
}

const searchInputEl = document.getElementById('search-input');
if (searchInputEl) {
    searchInputEl.addEventListener('input', renderTable);
}

// === HINZUFÜGEN & VORSCHAU PROZESS ===
const btnAddProduct = document.getElementById('btn-add-product');
if (btnAddProduct) {
    btnAddProduct.addEventListener('click', () => {
        const step1 = document.getElementById('add-step-1');
        const step2 = document.getElementById('add-step-2');
        if (step1) step1.style.display = 'block';
        if (step2) step2.style.display = 'none';
        
        const addEan = document.getElementById('add-ean');
        const addQty = document.getElementById('add-quantity');
        const addMhd = document.getElementById('add-mhd');
        if (addEan) addEan.value = '';
        if (addQty) addQty.value = '1';
        if (addMhd) addMhd.value = '';
        
        openModal('modal-add');
    });
}

async function fetchPreview() {
    const ean = document.getElementById('add-ean').value.trim();
    const qty = parseInt(document.getElementById('add-quantity').value);
    const mhd = document.getElementById('add-mhd').value;

    if (!ean || !mhd || isNaN(qty)) {
        alert('Bitte EAN, Menge und MHD ausfüllen!');
        return;
    }

    let productName = "Unbekanntes Produkt";
    let companyName = "Unbekannte Marke";

    try {
        const response = await fetch(`https://world.openfoodfacts.org/api/v2/product/${ean}.json`);
        if (response.ok) {
            const data = await response.json();
            if (data.status === 1 && data.product) {
                productName = data.product.product_name || productName;
                companyName = data.product.brands || data.product.brandProp || companyName;
            }
        }
    } catch (e) {
        console.warn("OpenFoodFacts nicht erreichbar, nutze Fallback");
    }

    document.getElementById('prev-name').value = productName;
    document.getElementById('prev-company').value = companyName;
    document.getElementById('prev-ean').value = ean;
    document.getElementById('prev-quantity').value = qty;
    document.getElementById('prev-price').value = "0.00";
    document.getElementById('prev-mhd').value = mhd;
    document.getElementById('prev-comment').value = "";

    document.getElementById('add-step-1').style.display = 'none';
    document.getElementById('add-step-2').style.display = 'block';
}

function rejectAdd() {
    closeModal('modal-add');
}

async function confirmAdd() {
    const ean = document.getElementById('prev-ean').value.trim();
    const qty = parseInt(document.getElementById('prev-quantity').value);
    const mhd = document.getElementById('prev-mhd').value;
    const name = document.getElementById('prev-name').value.trim();
    const company = document.getElementById('prev-company').value.trim();
    const price = parseFloat(document.getElementById('prev-price').value) || 0.0;
    const comment = document.getElementById('prev-comment').value.trim();

    const payload = {
        articleNumber: ean,
        quantity: qty,
        expirationDate: mhd,
        comment: comment
    };

    try {
        const res = await fetchWithAuth(API_BASE, {
            method: 'POST',
            body: JSON.stringify(payload)
        });

        if (!res.ok) throw new Error('Fehler beim Speichern');
        const createdItem = await res.json();

        // Optionales Update, falls Name/Preis/Kommentar in Vorschau angepasst wurden
        const updatedPayload = {
            productName: name || createdItem.productName,
            companyName: company || createdItem.companyName,
            quantity: qty,
            price: price,
            expirationDate: mhd,
            comment: comment
        };

        await fetchWithAuth(`${API_BASE}/${createdItem.id}`, {
            method: 'PUT',
            body: JSON.stringify(updatedPayload)
        });

        closeModal('modal-add');
        loadInventory();
    } catch (err) {
        alert('Fehler beim Speichern des Produkts.');
        console.error(err);
    }
}

// === BEARBEITEN PROZESS ===
function openEditModal(id) {
    const item = inventoryData.find(i => i.id === id);
    if (!item) return;

    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-name').value = item.productName || '';
    document.getElementById('edit-company').value = item.companyName || '';
    document.getElementById('edit-quantity').value = item.quantity || 1;
    document.getElementById('edit-price').value = item.price || 0;
    document.getElementById('edit-mhd').value = item.expirationDate ? item.expirationDate.split('T')[0] : '';
    document.getElementById('edit-comment').value = item.comment || '';

    openModal('modal-edit');
}

async function saveEdit() {
    const id = document.getElementById('edit-id').value;
    const payload = {
        productName: document.getElementById('edit-name').value,
        companyName: document.getElementById('edit-company').value,
        quantity: parseInt(document.getElementById('edit-quantity').value) || 1,
        price: parseFloat(document.getElementById('edit-price').value) || 0,
        expirationDate: document.getElementById('edit-mhd').value,
        comment: document.getElementById('edit-comment').value.trim()
    };

    try {
        const res = await fetchWithAuth(`${API_BASE}/${id}`, {
            method: 'PUT',
            body: JSON.stringify(payload)
        });

        if (!res.ok) throw new Error('Fehler beim Aktualisieren');

        closeModal('modal-edit');
        loadInventory();
    } catch (err) {
        alert('Fehler beim Aktualisieren.');
        console.error(err);
    }
}

// === LÖSCHEN PROZESS ===
function openDeleteModal(id, productName) {
    document.getElementById('delete-id').value = id;
    const promptText = document.getElementById('delete-prompt-text');
    if (promptText) {
        promptText.textContent = `Möchtest du das Produkt "${productName}" (ID: #${id}) wirklich unwiderruflich löschen?`;
    }
    openModal('modal-delete');
}

async function confirmDelete() {
    const id = document.getElementById('delete-id').value;

    try {
        const res = await fetchWithAuth(`${API_BASE}/${id}`, {
            method: 'DELETE'
        });

        if (!res.ok) throw new Error('Fehler beim Löschen');

        closeModal('modal-delete');
        loadInventory();
    } catch (err) {
        alert('Fehler beim Löschen des Produkts.');
        console.error(err);
    }
}

// === MODAL HILFSFUNKTIONEN ===
function openModal(id) {
    const el = document.getElementById(id);
    if (el) el.style.display = 'flex';
}

function closeModal(id) {
    const el = document.getElementById(id);
    if (el) el.style.display = 'none';
}

// Initialer Start
loadInventory();
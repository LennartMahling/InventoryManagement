# Inventarverwaltung

Eine webbasierte Inventarverwaltung zur Erfassung und Verwaltung von Lebensmitteln und Verbrauchsmaterialien.

Das Projekt wurde als praktische Softwarelösung für das **Technische Hilfswerk (THW)** entwickelt. Es besteht aus einem ASP.NET-Core-Backend auf Basis von **C# / .NET 10** und einem schlanken Frontend aus **HTML, CSS und JavaScript**.

Die Anwendung kann lokal ausgeführt oder auf einem Server bereitgestellt werden.

---

## Inhaltsverzeichnis

* [Über das Projekt](#über-das-projekt)
* [Funktionen](#funktionen)
* [Technologien](#technologien)
* [Projektstruktur](#projektstruktur)
* [Voraussetzungen](#voraussetzungen)
* [Installation](#installation)
* [Konfiguration](#konfiguration)
* [Anwendung starten](#anwendung-starten)
* [Verwendung](#verwendung)
* [API](#api)
* [Datenbank](#datenbank)
* [Open Food Facts](#open-food-facts)
* [Authentifizierung](#authentifizierung)
* [Deployment](#deployment)
* [Sicherheit](#sicherheit)
* [Entwicklung](#entwicklung)
* [Bekannte Einschränkungen](#bekannte-einschränkungen)
* [Lizenz](#lizenz)

---

# Über das Projekt

Die Inventarverwaltung ermöglicht die digitale Erfassung und Verwaltung von Produkten innerhalb eines Inventars.

Beim Anlegen eines Produkts kann eine **EAN bzw. Artikelnummer** eingegeben werden. Die Anwendung versucht anschließend, über die öffentliche **Open-Food-Facts-API** automatisch Informationen wie Produktname und Hersteller bzw. Marke abzurufen.

Die Daten können anschließend kontrolliert und angepasst werden, bevor das Produkt im Inventar gespeichert wird.

Zu jedem Produkt werden unter anderem folgende Informationen gespeichert:

* ID
* EAN / Artikelnummer
* Produktname
* Hersteller / Marke
* Menge
* Preis
* Mindesthaltbarkeitsdatum (MHD)
* Kommentar

Das Frontend und Backend werden gemeinsam von der ASP.NET-Core-Anwendung ausgeliefert. Dadurch ist keine separate Frontend-Anwendung oder JavaScript-Laufzeitumgebung erforderlich.

---

# Funktionen

## Inventarverwaltung

* Produkte hinzufügen
* Produkte bearbeiten
* Produkte löschen
* Produktmenge verwalten
* Preis hinterlegen
* Mindesthaltbarkeitsdatum verwalten
* Kommentare zu Produkten speichern

## Produkterkennung

Bei der Erfassung eines neuen Produkts kann anhand der EAN automatisch versucht werden, Produktinformationen über Open Food Facts abzurufen.

Die abgerufenen Informationen werden vor dem Speichern als Vorschau angezeigt und können vom Benutzer angepasst werden.

## Suche

Das Inventar kann nach verschiedenen Informationen durchsucht werden:

* ID
* EAN / Artikelnummer
* Produktname
* Hersteller / Marke
* Menge
* Preis
* MHD
* Kommentar

## MHD-Übersicht

Für jedes Produkt wird automatisch berechnet, wie viele Tage bis zum Mindesthaltbarkeitsdatum verbleiben.

Produkte werden abhängig vom verbleibenden Zeitraum entsprechend gekennzeichnet:

* **abgelaufen**
* **MHD innerhalb der nächsten 14 Tage**
* **mehr als 14 Tage verbleibend**

## Authentifizierung

Die API ist durch eine JWT-basierte Authentifizierung geschützt.

Ohne gültiges Login können keine Inventardaten abgerufen oder verändert werden.

---

# Technologien

## Backend

* **C#**
* **.NET 10**
* **ASP.NET Core Minimal APIs**
* **Entity Framework Core**
* **SQLite**
* **JWT Bearer Authentication**
* **BCrypt**

## Frontend

* HTML5
* CSS3
* JavaScript

## Externe Dienste

* [Open Food Facts](https://world.openfoodfacts.org/) zur automatischen Produkterkennung anhand der EAN

## Entwicklung

Das Projekt kann beispielsweise mit folgenden Entwicklungsumgebungen bearbeitet werden:

* JetBrains Rider
* Visual Studio
* Visual Studio Code

---

# Projektstruktur

Die grundlegende Struktur des Repositorys sieht folgendermaßen aus:

```text
InventoryManagement/
│
├── BE_InventoryManagement/
│   │
│   ├── wwwroot/
│   │   ├── index.html
│   │   ├── app.js
│   │   ├── style.css
│   │   └── logo-thw.svg
│   │
│   ├── Properties/
│   │   └── launchSettings.json
│   │
│   ├── Program.cs
│   ├── Product.cs
│   ├── InventoryContext.cs
│   ├── BE_InventoryManagement.csproj
│   └── ...
│
├── BE_InventoryManagement.sln
├── .gitignore
└── README.md
```

### Backend

`Program.cs` enthält die Konfiguration der ASP.NET-Core-Anwendung sowie die API-Endpunkte.

`Product.cs` definiert das Datenmodell eines Inventarprodukts.

`InventoryContext.cs` konfiguriert Entity Framework Core und die SQLite-Datenbank.

### Frontend

Der Ordner `wwwroot` enthält die statischen Dateien der Weboberfläche.

```text
wwwroot/
├── index.html
├── app.js
├── style.css
└── logo-thw.svg
```

Da ASP.NET Core statische Dateien aus `wwwroot` ausliefert, kann das Frontend direkt über dieselbe Anwendung bereitgestellt werden.

---

# Voraussetzungen

Für die lokale Entwicklung werden benötigt:

* **.NET 10 SDK**
* Git
* optional eine IDE wie JetBrains Rider oder Visual Studio

Das installierte .NET SDK kann mit folgendem Befehl überprüft werden:

```bash
dotnet --version
```

Es sollte eine Version aus dem Bereich `.NET 10` angezeigt werden.

---

# Installation

## 1. Repository klonen

Das Repository kann beispielsweise mit Git geklont werden:

```bash
git clone https://github.com/LennartMahling/InventoryManagement.git
```

Anschließend in das Projektverzeichnis wechseln:

```bash
cd InventoryManagement
```

---

## 2. Abhängigkeiten wiederherstellen

Die benötigten NuGet-Pakete werden mit folgendem Befehl installiert bzw. wiederhergestellt:

```bash
dotnet restore
```

Die wichtigsten verwendeten Pakete sind unter anderem:

* `Microsoft.EntityFrameworkCore.Sqlite`
* `Microsoft.AspNetCore.Authentication.JwtBearer`
* `Microsoft.AspNetCore.OpenApi`
* `BCrypt.Net-Next`

Die konkreten Versionen sind in der Projektdatei `BE_InventoryManagement.csproj` definiert.

---

# Konfiguration

## authsettings.json

Die Anwendung benötigt eine lokale Konfigurationsdatei namens:

```text
authsettings.json
```

Diese Datei wird aus Sicherheitsgründen nicht in Git gespeichert.

Die Anwendung lädt die Datei beim Start und erwartet darin die Zugangsdaten sowie den geheimen Schlüssel für die JWT-Authentifizierung.

Erstelle:

```text
BE_InventoryManagement/authsettings.json
```

mit folgendem Aufbau:

```json
{
  "Auth": {
    "Username": "admin",
    "PasswordHash": "BCRYPT_HASH",
    "JwtSecret": "YOUR_LONG_RANDOM_SECRET"
  }
}
```

### Bedeutung der Werte

| Einstellung    | Beschreibung                                    |
| -------------- | ----------------------------------------------- |
| `Username`     | Benutzername für die Anmeldung                  |
| `PasswordHash` | BCrypt-Hash des Passworts                       |
| `JwtSecret`    | Geheimer Schlüssel zum Signieren der JWT-Tokens |

### Wichtig

**Das tatsächliche Passwort darf nicht direkt in `authsettings.json` gespeichert werden.**

Stattdessen wird ein BCrypt-Hash benötigt.

Ein BCrypt-Hash kann beispielsweise mit einem kleinen C#-Programm erzeugt werden:

```csharp
using BCrypt.Net;

Console.WriteLine(
    BCrypt.Net.BCrypt.HashPassword("DEIN_PASSWORT")
);
```

Der ausgegebene Hash wird anschließend als `PasswordHash` eingetragen.

Beispiel:

```json
{
  "Auth": {
    "Username": "admin",
    "PasswordHash": "$2a$11$...",
    "JwtSecret": "ein-langer-zufaelliger-geheimer-schluessel"
  }
}
```

Der `JwtSecret` sollte ausreichend lang und zufällig sein.

**`authsettings.json` niemals in ein öffentliches Git-Repository committen.**

Die Datei ist deshalb bereits in `.gitignore` eingetragen.

---

# Anwendung starten

## Variante 1 – .NET CLI

Vom Repository-Root aus:

```bash
dotnet run --project BE_InventoryManagement
```

Alternativ kann direkt in das Backend-Verzeichnis gewechselt werden:

```bash
cd BE_InventoryManagement
dotnet run
```

Die Anwendung verwendet in der Entwicklungsumgebung standardmäßig die in `Properties/launchSettings.json` definierten URLs.

Aktuell sind dort unter anderem folgende Adressen konfiguriert:

```text
http://localhost:5258
https://localhost:7146
```

Die HTTPS-Adresse kann bei einer lokalen Entwicklung eine entsprechende ASP.NET-Core-Entwicklungsumgebung bzw. ein lokales Entwicklungszertifikat benötigen.

---

# Anwendung aufrufen

Nach dem Start kann die Anwendung im Browser geöffnet werden.

Beispielsweise:

```text
http://localhost:5258
```

oder:

```text
https://localhost:7146
```

Beim ersten Aufruf wird der Login angezeigt.

Nach erfolgreicher Anmeldung wird das Inventar geladen.

---

# Verwendung

## Produkt hinzufügen

1. Auf **„Produkt hinzufügen“** klicken.
2. EAN bzw. Artikelnummer eingeben.
3. Menge eingeben.
4. MHD auswählen.
5. **„Vorschau laden“** auswählen.

Die Anwendung versucht anschließend, anhand der EAN Produktinformationen von Open Food Facts abzurufen.

Anschließend können die Daten kontrolliert und angepasst werden.

Mit:

**„Bestätigen & Speichern“**

wird das Produkt im Inventar gespeichert.

---

## Produkt bearbeiten

Über den Button **„Bearbeiten“** kann ein vorhandener Datensatz geändert werden.

Änderbar sind unter anderem:

* Produktname
* Hersteller
* Menge
* Preis
* MHD
* Kommentar

Die Änderungen werden direkt in der SQLite-Datenbank gespeichert.

---

## Produkt löschen

Über **„Löschen“** kann ein Produkt aus dem Inventar entfernt werden.

Vor dem endgültigen Löschen erscheint eine Bestätigungsabfrage.

Das Löschen ist nicht automatisch rückgängig zu machen.

---

## Produkte suchen

Das Suchfeld durchsucht die vorhandenen Inventardaten.

Unter anderem kann nach folgenden Informationen gesucht werden:

```text
Produktname
EAN
ID
Hersteller
Menge
Preis
MHD
Kommentar
```

---

# API

Das Backend stellt eine REST-ähnliche HTTP-API über ASP.NET Core Minimal APIs bereit.

Die API befindet sich unter:

```text
/api
```

## Login

### `POST /api/login`

Authentifiziert einen Benutzer und gibt ein JWT zurück.

Request:

```json
{
  "username": "admin",
  "password": "DEIN_PASSWORT"
}
```

Bei erfolgreicher Anmeldung wird beispielsweise folgende Antwort zurückgegeben:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

Das Frontend speichert dieses Token lokal im Browser und verwendet es anschließend für geschützte API-Anfragen.

---

## Inventar abrufen

### `GET /api/inventory`

Gibt alle Inventareinträge zurück.

Authentifizierung erforderlich:

```http
Authorization: Bearer <JWT>
```

---

## Produkt hinzufügen

### `POST /api/inventory`

Erstellt einen neuen Inventareintrag.

Beispiel:

```json
{
  "articleNumber": "4001234567890",
  "quantity": 5,
  "expirationDate": "2027-05-31",
  "comment": "Fach A, Kiste 3"
}
```

Wird bereits ein Produkt mit derselben Kombination aus:

```text
ArticleNumber + ExpirationDate
```

gefunden, wird dessen Menge erhöht, anstatt einen weiteren Datensatz mit derselben Kombination anzulegen.

---

## Produkt bearbeiten

### `PUT /api/inventory/{id}`

Aktualisiert einen vorhandenen Inventareintrag.

Beispiel:

```json
{
  "productName": "Beispielprodukt",
  "companyName": "Beispielhersteller",
  "quantity": 10,
  "price": 0.0,
  "expirationDate": "2027-05-31",
  "comment": "Aktualisierter Kommentar"
}
```

---

## Produkt löschen

### `DELETE /api/inventory/{id}`

Löscht einen Inventareintrag anhand seiner ID.

---

# Datenbank

Die Anwendung verwendet **SQLite**.

Die Datenbankdatei heißt:

```text
inventory.db
```

Sie wird im Anwendungsverzeichnis angelegt.

Die Anwendung verwendet Entity Framework Core zur Verwaltung der Datenbank.

Beim Start wird automatisch geprüft, ob die Datenbank aktualisiert werden muss:

```csharp
db.Database.Migrate();
```

Dadurch werden vorhandene Entity-Framework-Migrationen beim Start angewendet.

Eine manuelle Ausführung von `dotnet ef database update` ist für den normalen Start der Anwendung daher nicht erforderlich.

---

## Datenmodell

Ein Inventareintrag besitzt folgende Eigenschaften:

| Eigenschaft      | Typ        | Beschreibung             |
| ---------------- | ---------- | ------------------------ |
| `Id`             | `int`      | Eindeutige ID            |
| `ArticleNumber`  | `string`   | EAN / Artikelnummer      |
| `ProductName`    | `string`   | Produktname              |
| `CompanyName`    | `string`   | Hersteller / Marke       |
| `Quantity`       | `int`      | Anzahl                   |
| `Price`          | `decimal`  | Preis                    |
| `ExpirationDate` | `DateTime` | Mindesthaltbarkeitsdatum |
| `Comment`        | `string`   | Optionaler Kommentar     |

Die Kombination aus Artikelnummer und MHD ist eindeutig.

Dadurch können beispielsweise zwei Chargen desselben Produkts mit unterschiedlichen MHDs separat verwaltet werden.

---

# Open Food Facts

Beim Anlegen eines Produkts kann die EAN verwendet werden, um Produktinformationen automatisch abzurufen.

Dafür wird die öffentliche API von **Open Food Facts** verwendet.

Beispiel:

```text
https://world.openfoodfacts.org/api/v2/product/{EAN}.json
```

Abgerufen werden insbesondere:

* Produktname
* Marke / Hersteller

Die Daten werden anschließend als Vorschlag angezeigt.

Der Benutzer kann die Informationen vor dem Speichern ändern.

---

## Wenn Open Food Facts nicht erreichbar ist

Die Anwendung ist nicht vollständig von Open Food Facts abhängig.

Wenn kein Produkt gefunden wird oder die API nicht erreichbar ist, werden Fallback-Werte verwendet:

```text
Produktname: Unbekanntes Produkt
Hersteller: Unbekannte Marke
```

Das Produkt kann anschließend manuell angepasst werden.

---

# Authentifizierung

Die Anwendung verwendet **JWT (JSON Web Tokens)**.

Der Ablauf ist:

```text
Benutzer
   │
   │ Benutzername + Passwort
   ▼
POST /api/login
   │
   │ Prüfung der Zugangsdaten
   ▼
JWT Token
   │
   │ Authorization: Bearer <Token>
   ▼
Geschützte API-Endpunkte
```

Die geschützten Inventar-Endpunkte akzeptieren nur Anfragen mit einem gültigen JWT.

Die Gültigkeitsdauer des Tokens beträgt aktuell **7 Tage**.

Das Frontend speichert das Token im Browser in:

```text
localStorage
```

unter dem Schlüssel:

```text
thw_jwt_token
```

---

# Deployment

Die Anwendung kann grundsätzlich auf einem Server mit installiertem .NET 10 Runtime bzw. SDK betrieben werden.

Für ein Produktionsdeployment empfiehlt sich, die Anwendung zunächst zu veröffentlichen:

```bash
dotnet publish BE_InventoryManagement/BE_InventoryManagement.csproj \
    -c Release \
    -o publish
```

Anschließend befindet sich die veröffentlichte Anwendung im Ordner:

```text
publish/
```

Die Anwendung kann dort beispielsweise mit:

```bash
dotnet BE_InventoryManagement.dll
```

gestartet werden.

Für einen produktiven Linux-Server empfiehlt sich zusätzlich die Verwendung eines Process Managers wie `systemd` sowie eines Reverse Proxys wie Apache oder nginx.

Ein typischer Aufbau wäre:

```text
Internet
    │
    ▼
HTTPS
    │
    ▼
Apache / nginx
    │
    ▼
ASP.NET Core / Kestrel
    │
    ├── wwwroot
    ├── REST API
    └── SQLite
```

Die genaue Serverkonfiguration hängt von der verwendeten Infrastruktur ab.

---

# Sicherheit

## Secrets

Folgende Informationen dürfen nicht öffentlich in Git gespeichert werden:

* Passwörter
* BCrypt-Hashes, wenn sie für produktive Zugangsdaten verwendet werden
* JWT-Secrets
* andere private Zugangsdaten

Die Datei:

```text
authsettings.json
```

ist deshalb in `.gitignore` ausgeschlossen.

---

## HTTPS

Bei einem produktiven Betrieb sollte die Anwendung ausschließlich über HTTPS erreichbar sein.

Insbesondere Login-Daten und JWT-Tokens sollten nicht über eine unverschlüsselte HTTP-Verbindung übertragen werden.

---

## JWT

JWT-Tokens ermöglichen den Zugriff auf die geschützten API-Endpunkte.

Der verwendete `JwtSecret` muss daher geheim bleiben.

Bei einer Kompromittierung des Secrets sollten bestehende Tokens als kompromittiert betrachtet und das Secret ersetzt werden.

---

# Entwicklung

## Projekt öffnen

Das Projekt kann beispielsweise in JetBrains Rider geöffnet werden.

Die Solution befindet sich im Repository-Root:

```text
BE_InventoryManagement.sln
```

Nach dem Öffnen sollten die NuGet-Abhängigkeiten automatisch wiederhergestellt werden.

Alternativ:

```bash
dotnet restore
```

---

## Anwendung im Entwicklungsmodus starten

```bash
dotnet run --project BE_InventoryManagement
```

Die Entwicklungs-URLs sind in:

```text
BE_InventoryManagement/Properties/launchSettings.json
```

definiert.

---

## Build überprüfen

Ein Release-Build kann mit folgendem Befehl erstellt werden:

```bash
dotnet build -c Release
```

Für ein deploybares Paket:

```bash
dotnet publish BE_InventoryManagement/BE_InventoryManagement.csproj \
    -c Release \
    -o publish
```

---

# Änderungen an der Datenbank

Wenn das Datenmodell geändert wird, sollte eine neue Entity-Framework-Core-Migration erstellt werden.

Beispielsweise:

```bash
dotnet ef migrations add BeschreibungDerAenderung \
    --project BE_InventoryManagement
```

Anschließend kann die Anwendung mit:

```bash
dotnet run --project BE_InventoryManagement
```

gestartet werden.

Die Anwendung wendet vorhandene Migrationen beim Start automatisch an.

> Hinweis: Für die Erstellung von Migrationen muss gegebenenfalls das Entity-Framework-Core-CLI-Tool installiert sein.

---

# Bekannte Einschränkungen

Das Projekt ist primär als praktische Inventarverwaltung für einen konkreten Anwendungsfall entwickelt worden.

Aktuell gibt es unter anderem folgende Einschränkungen:

* SQLite ist für eine einzelne bzw. kleine Anwendung ausgelegt und nicht als zentrale Datenbank für große parallele Benutzerzahlen gedacht.
* Es gibt aktuell nur einen konfigurierten Benutzer.
* Die Benutzerverwaltung ist nicht als separates System implementiert.
* Es gibt keine Rollen- oder Rechteverwaltung.
* Produktinformationen hängen von der Verfügbarkeit der Open-Food-Facts-Daten ab.
* Der Preis wird bei der automatischen Produkterkennung nicht aus Open Food Facts übernommen und standardmäßig mit `0,00 €` angelegt.
* JWT-Tokens werden clientseitig im `localStorage` gespeichert.
* Es existiert aktuell kein integriertes Backup-System für die SQLite-Datenbank.
* Das Löschen eines Produkts ist dauerhaft.

---

# Mögliche zukünftige Erweiterungen

Mögliche Erweiterungen des Projekts wären beispielsweise:

* Barcode-Scanner-Unterstützung
* EAN-Erkennung über die Smartphone-Kamera
* mehrere Benutzer
* Rollen und Berechtigungen
* Änderungsverlauf
* automatisierte Datenbank-Backups
* CSV-/Excel-Export
* Import bestehender Inventardaten
* Lagerorte
* Bestandswarnungen
* detailliertere Auswertungen
* bessere mobile Darstellung
* zentrale Benutzerverwaltung
* Protokollierung von Änderungen

---

# Lizenz

Dieses Projekt wurde als praktische Softwarelösung für das THW entwickelt.

Falls das Projekt öffentlich weiterverwendet, verändert oder verteilt werden soll, sollte vor einer Veröffentlichung eine geeignete Lizenz im Repository ergänzt werden.

---

# Autor

**Lennart Mahling**

GitHub:

https://github.com/LennartMahling

Repository:

https://github.com/LennartMahling/InventoryManagement

---

## Hinweis zur Nutzung

Das Projekt dient der digitalen Verwaltung von Inventardaten.

Bei einem produktiven Einsatz sollten insbesondere Datenschutz, Zugriffsschutz, Backups und die sichere Verwaltung von Zugangsdaten entsprechend der jeweiligen organisatorischen Anforderungen berücksichtigt werden.

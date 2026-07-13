# 🏢 Corporate Helpdesk

> Kompleksowa aplikacja webowa do zarządzania zgłoszeniami wsparcia IT i incydentami w środowisku korporacyjnym, która jeszcze jest/będzie w trakcie rozwoju.

## 📖 O projekcie

Corporate Helpdesk to autorskie rozwiązanie oparte na wzorcu MVC, zaprojektowane w celu usprawnienia procesu zgłaszania i rozwiązywania problemów technicznych w organizacji. Umożliwia pracownikom łatwe tworzenie zgłoszeń (ticketów), a zespołowi IT dostarcza przejrzysty panel (dashboard) do zarządzania, śledzenia i analizowania incydentów w czasie rzeczywistym. 

Projekt ten powstał, aby połączyć moje doświadczenie z pracy na pierwszej linii wsparcia (Helpdesk) z umiejętnościami programowania w .NET, kładąc nacisk na czystą architekturę i praktyczne zastosowanie biznesowe.

## ✨ Główne funkcjonalności

* **Autoryzacja oparta na rolach (RBAC):** Bezpieczne logowanie i autoryzacja przy użyciu ASP.NET Core Identity (Role: Admin, Pracownik).
* **System zarządzania zgłoszeniami:** Płynne tworzenie, edytowanie, kategoryzowanie i zamykanie zgłoszeń technicznych.
* **Interaktywny Dashboard:** Dynamiczne statystyki i wizualizacja danych z wykorzystaniem biblioteki Chart.js.
* **Zintegrowana komunikacja:** Wbudowany system komentarzy dla każdego zgłoszenia, ułatwiający komunikację na linii Admin-Pracownik.
* **Responsywny interfejs (UI):** W pełni responsywny wygląd oparty na modułowym SCSS (wzorzec 7-1), zapewniający wygodną obsługę na każdym urządzeniu.

## 🛠️ Technologie

* **Backend:** C#, .NET 8.0, ASP.NET Core MVC
* **Baza danych:** MS SQL Server, Entity Framework Core (podejście Code-First)
* **Frontend:** HTML5, SCSS, JavaScript, Chart.js
* **Bezpieczeństwo:** ASP.NET Core Identity

## Poglądowe zdjęcia
<img width="1876" height="932" alt="Image" src="https://github.com/user-attachments/assets/ae4898e4-2914-4b24-b482-7103f6800a2f" />
<img width="1877" height="936" alt="Image" src="https://github.com/user-attachments/assets/f497dd5c-f475-47b7-81fc-e5f4f184cdaa" />
<img width="1875" height="931" alt="Image" src="https://github.com/user-attachments/assets/933d1a64-9656-4143-ae4f-33862b6dd30f" />
<img width="1876" height="931" alt="Image" src="https://github.com/user-attachments/assets/c1cd3960-20fb-4395-8198-01e25065a6e5" />

## 🚀 Uruchomienie lokalne

Aby uruchomić projekt na swoim komputerze, wykonaj poniższe kroki.

### Wymagania

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (np. SQL Server Express lub LocalDB)
* Visual Studio 2022

### Instalacja

1. Sklonuj repozytorium:
   ```sh
   git clone [https://github.com/chomiczo/Corporate_Helpdesk.git](https://github.com/chomiczo/Corporate_Helpdesk.git)

# Projekt na przedmiot komunikacja człowiek-komputer

# Jak uruchomić aplikację

Aby uruchomić i korzystać z aplikacji wymagane jest:
- Visual Studio 2022
- SQL Server
- Microsoft SQL Server Management Studio
- .NET 8.0

Po uruchomieniu projektu z rozszerzeniem `.sln` w programie Visual Studio 2022 należy otworzyć plik `appsettings.json` i zmienić w nim `DefaultConnection`. Domyślnie aplikacja jest przygotowana do działania na serwerze lokalnym.

W pliku `appsettings.json` należy również podać `ClientID` oraz `AccessToken`

Następnie należy otworzyć menadżer pakietów NuGet i wpisać w nim komendę `update-database` w celu utworzenia bazy danych.

## Funkcjonalności aplikacji

MVP aplikacji oferuje następujące funkcjonalności:
1. Pobranie kolekcji gier z API IGDB.
2. Wyszukiwanie gier.
3. Tworzenie i zarządzanie osobistą listą gier.
4. Przeglądanie szczegółowych danych o każdej grze.
5. Logowanie, rejestracja i zarządzanie swoim kontem.

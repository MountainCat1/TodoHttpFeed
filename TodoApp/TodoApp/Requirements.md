Etapy:
1. Dokończenie Aplikacji "TODO CRUD API": Ukończ implementację aplikacji "TODO
   CRUD API", która służy do przechowywania elementów listy zadań do wykonania.
   Ten krok był omawiany podczas pierwszego spotkania rekrutacyjnego.
2. Stworzenie Aplikacji CMD: Stwórz aplikację konsolową (CMD), która co 10 sekund
   będzie pobierać listę elementów "ToDoItem" z "TODO CRUD API" i zapisywać je do
   pliku "ToDoItemsCurrentSnapshot.json" w katalogu roboczym aplikacji CMD.
3. Dodanie HTTP Feed dla "ToDoItem" w Aplikacji "TODO CRUD API":
* Zaimplementuj HTTP Feed dla "ToDoItem" w aplikacji "TODO CRUD API".
* Specyfikacja do implementacji: skorzystaj z https://www.http-feeds.org/,
* bez uwierzytelniania, bez cachowania, bazując na long-pollingu. Jako ID
   można użyć zwykłego GUID-a.

4. Modyfikacja Aplikacji CMD z Kroku 2.: Zaktualizuj aplikację CMD, aby korzystała z
   feeda do ciągłej aktualizacji pliku "ToDoItemsCurrentSnapshot.json".
5. Umieszczenie Aplikacji na GitHub lub Innym Publicznie Dostępnym Repozytorium:
   Udostępnij aplikacje na platformie GitHub lub innym publicznie dostępnym
   repozytorium kodu. Podziel się linkiem do repozytorium.
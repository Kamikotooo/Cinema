using System;
using System.IO;

class Program
{
    // Переменные для хранения информации о билете
    static string selectedMovie = "";
    static string selectedSession = "";
    static string selectedSeat = "";
    static decimal ticketPrice = 0m;
    static int ticketQuantity = 0;

    static void Main()
    {
        EnsureFilesExist(); // Проверяем/создаем необходимые файлы
        while (true)
        {
            Console.Clear();
            DrawBox("Кинотеатр - Главное меню", 2, 2, 60, 10);
            Console.SetCursorPosition(4, 4);
            string[] mainMenuOptions = {
                "Вход в модуль 'Администратор'",
                "Вход в модуль 'Кассир'",
                "Вход в модуль 'Афиша'",
                "Выход"
            };
            int currentSelection = 0;
            int menuLength = mainMenuOptions.Length;
            while (true)
            {
                for (int i = 0; i < menuLength; i++)
                {
                    Console.SetCursorPosition(4, 4 + i);
                    if (i == currentSelection)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(mainMenuOptions[i]);
                    Console.ResetColor();
                }
                Console.SetCursorPosition(4, 4 + menuLength + 1);
                ConsoleKey key = Console.ReadKey(true).Key;//скрывает символ, который пользователь нажал. .Key – получаем конкретную клавишу.
                // Раскладка перемещения навигации по стрелкам
                if (key == ConsoleKey.UpArrow)
                {
                    currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;//(условие) ? если_да : если_нет – тернарный оператор (короткая форма if...else).
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
                }
                //Проверка выбора клавишей Enter + кейс
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentSelection)
                    {
                        case 0:
                            AuthenticateAndEnterModule("Администратор", AdminModule);
                            break;
                        case 1:
                            AuthenticateAndEnterModule("Кассир", CashierModule);
                            break;
                        case 2:
                            PosterModule();
                            break;
                        case 3:
                            return;
                    }
                    break;
                }
            }
        }
    }

    // Создаем файлы, если не созданы, если файлы созданы, они не дублируются, информация не стрирается
    static void EnsureFilesExist()
    {
        if (!File.Exists("users.txt")) File.Create("users.txt").Close();
        if (!File.Exists("movies.txt")) File.Create("movies.txt").Close();
        if (!File.Exists("sessions.txt")) File.Create("sessions.txt").Close();
        if (!File.Exists("seats.txt")) File.Create("seats.txt").Close();
        if (!File.Exists("tickets.txt")) File.Create("tickets.txt").Close();
    }

    // Отрисовка рамки + заголовок по центру верхней границы
    static void DrawBox(string title, int x, int y, int width, int height)
    {
        Console.SetCursorPosition(x, y);
        Console.Write("╔" + new string('═', width - 2) + "╗");
        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write("║" + new string(' ', width - 2) + "║");
        }
        Console.SetCursorPosition(x, y + height - 1);
        Console.Write("╚" + new string('═', width - 2) + "╝");
        if (!string.IsNullOrEmpty(title))
        {
            Console.SetCursorPosition(x + (width - title.Length) / 2, y);
            Console.Write(title);
        }
    }

    // Сообщение с цветом и ожиданием нажатия клавиши
    static void DisplayMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
        Console.ReadKey();
    }

    // Авторизация пользователя + запрос лого и пароля 
    static void AuthenticateAndEnterModule(string moduleName, Action moduleAction)
    {
        Console.Clear();//очистка экрана
        DrawBox("Авторизация - " + moduleName, 2, 2, 60, 10);
        Console.SetCursorPosition(4, 4);
        Console.Write("Введите логин: ");
        string login = Console.ReadLine();
        Console.SetCursorPosition(4, 5);
        Console.Write("Введите пароль: ");
        string password = ReadPassword();
        if (AuthenticateUser(login, password, moduleName))//Проверяет данные через AuthenticateUser(). Если всё верно — запускает нужный модуль.
        {
            moduleAction();
        }
        else
        {
            DisplayMessage("Неверный логин или пароль.", ConsoleColor.Red);
        }
    }

    // Считывание пароля с маской
    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)//Считывает каждый символ, кроме Enter и Backspace
            {
                password += key.KeyChar;//Добавляет его в переменную
                Console.Write("*");//Показывает *
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)//Если нажат Backspace — удаляет последний символ.
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);//обработка Enter
        Console.WriteLine();
        return password;
    }

    // Проверка логина, пароля и роли
    static bool AuthenticateUser(string login, string password, string moduleName)
    {
        string[] lines = File.ReadAllLines("users.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 3 && parts[0] == login && parts[1] == password && parts[2] == moduleName)//Разбивает каждую строку на части по запятой: login,password,module + проверка на запись 
            {
                return true;
            }
        }
        return false;
    }

    // Админ-модуль
    static void AdminModule()
    {
        while (true)
        {
            Console.Clear();
            DrawBox("Модуль 'Администратор'", 2, 2, 60, 15);
            string[] adminMenuOptions = {
                "Просмотр списка пользователей",
                "Добавление нового пользователя",
                "Редактирование данных пользователя",
                "Удаление пользователя",
                "Редактирование фильма",
                "Вернуться в главное меню"
            };
            int currentSelection = 0;
            int menuLength = adminMenuOptions.Length;
            while (true)
            {
                for (int i = 0; i < menuLength; i++)
                {
                    Console.SetCursorPosition(4, 4 + i);
                    if (i == currentSelection)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(adminMenuOptions[i]);
                    Console.ResetColor();
                }
                Console.SetCursorPosition(4, 4 + menuLength + 1);
                Console.Write("Выберите действие: ");
                ConsoleKey key = Console.ReadKey(true).Key;//скрывает символ, который пользователь нажал. .Key – получаем конкретную клавишу.
                // Раскладка перемещения навигации по стрелкам
                if (key == ConsoleKey.UpArrow)
                {
                    currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;////(условие) ? если_да : если_нет – тернарный оператор (короткая форма if...else).
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
                }
                else if (key == ConsoleKey.Enter)//Проверка выбора клавишей Enter + кейс
                {
                    switch (currentSelection)
                    {
                        case 0:
                            ViewUsers();
                            break;
                        case 1:
                            AddUser();
                            break;
                        case 2:
                            EditUser();
                            break;
                        case 3:
                            DeleteUser();
                            break;
                        case 4:
                            EditMovie();
                            break;
                        case 5:
                            return;
                    }
                    break;
                }
            }
        }
    }

    // Редактирование фильма
    static void EditMovie()
    {
        Console.Clear();
        DrawBox("Редактирование фильма", 2, 2, 60, 15);

        string filePath = "movies.txt";//Задаем путь

        if (!File.Exists(filePath))//Проверяем, существует ли файл
        {
            DisplayMessage("Файл 'movies.txt' не найден.", ConsoleColor.Red);
            return;
        }

        string[] movies = File.ReadAllLines(filePath);//Считываем все строки
        //проверка(обработка ) наличия фильма
        if (movies.Length == 0)
        {
            DisplayMessage("Фильмы отсутствуют.", ConsoleColor.Yellow);
            return;
        }

        int currentSelection = 0;//currentSelection — индекс текущего выбранного фильма.
        int menuLength = movies.Length;//menuLength — количество фильмов в списке.
        //Замена фильма на новый
        Console.SetCursorPosition(4, 4);
        Console.WriteLine("Выберите фильм, который хотите заменить:");

        while (true)
        {
            for (int i = 0; i < menuLength; i++)
            {
                Console.SetCursorPosition(4, 6 + i);
                if (i == currentSelection)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;//Выбранный фильм выделяется серым фоном.
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine((i + 1) + ". " + movies[i]);
                Console.ResetColor();// сброс цвета.
            }

            Console.SetCursorPosition(4, 6 + menuLength + 1);
            Console.Write("Используйте стрелки вверх/вниз и Enter для выбора");

            var key = Console.ReadKey(true).Key;//Считывает нажатую клавишу без отображения символа на экране. .Key — получаем конкретную клавишу(UpArrow, DownArrow, Enter).

            if (key == ConsoleKey.UpArrow)
            {
                currentSelection = currentSelection == 0 ? menuLength - 1 : currentSelection - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentSelection = currentSelection == menuLength - 1 ? 0 : currentSelection + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                string selectedMovie = movies[currentSelection];//Сохраняем выбранный фильм в переменную selectedMovie.

                Console.Clear();
                DrawBox("Изменение фильма", 2, 2, 60, 10);
                Console.SetCursorPosition(4, 4);
                Console.Write("Введите новый фильм для проката: ");
                string newMovie = Console.ReadLine().Trim();//.Trim() — удаляет лишние пробелы в начале и конце строки.

                // Удаляем лишние символы
                newMovie = newMovie.Replace(':', ' ').Trim();//Убираем символ : (если он был), заменяя его на пробел.

                // Обновляем фильм Ищем в массиве movies старое название.Заменяем его на новое. Прерываем цикл после замены.
                for (int i = 0; i < movies.Length; i++)
                {
                    if (movies[i] == selectedMovie)
                    {
                        movies[i] = newMovie;
                        break;
                    }
                }

                File.WriteAllLines(filePath, movies);
                DisplayMessage("Фильм успешно изменён!", ConsoleColor.Green);
                return;
            }
        }
    }

    // Просмотр пользователей
    static void ViewUsers()
    {
        Console.Clear();
        DrawBox("Список пользователей", 2, 2, 60, 15);
        string[] users = File.ReadAllLines("users.txt");//Читает список пользователей из файла.
        if (users.Length == 0)//Если нет записей — выводит сообщение.
        {
            DisplayMessage("Список пользователей пуст.", ConsoleColor.Yellow);
            return;
        }
        for (int i = 0; i < users.Length; i++)//Выводит всех пользователей построчно.
        {
            string[] userData = users[i].Split(',');
            Console.SetCursorPosition(4, 4 + i);
            Console.WriteLine("Логин: " + userData[0] + ", Роль: " + userData[2]);
        }
        Console.SetCursorPosition(4, 4 + users.Length + 1);
        Console.WriteLine("Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    // Добавление пользователя
    static void AddUser()
    {
        Console.Clear();
        DrawBox("Добавление пользователя", 2, 2, 60, 15);
        Console.SetCursorPosition(4, 4);
        Console.Write("Введите логин нового пользователя: ");
        string login = Console.ReadLine();
        Console.SetCursorPosition(4, 5);
        Console.Write("Введите пароль нового пользователя: ");
        string password = ReadPassword();
        Console.SetCursorPosition(4, 6);
        Console.Write("Введите роль пользователя (например, 'Кассир' или 'Администратор'): ");
        string role = Console.ReadLine();
        string newUser = login + "," + password + "," + role;//Правильная запись и сохранение в переменной
        File.AppendAllLines("users.txt", new string[] { newUser });//запись в файл в правильном виде
        DisplayMessage("Пользователь успешно добавлен.", ConsoleColor.Green);
    }

    // Редактирование пользователя
    static void EditUser()
    {
        Console.Clear();
        DrawBox("Редактирование пользователя", 2, 2, 60, 15);
        Console.SetCursorPosition(4, 4);
        Console.Write("Введите логин пользователя, которого нужно отредактировать: ");
        string login = Console.ReadLine();
        string[] users = File.ReadAllLines("users.txt");//Считывает все строки из файла users.txt в массив строк users. Каждая строка имеет формат: login,password,module

        bool userFound = false;//Переменная для проверки: найден ли пользователь по введённому логину
        for (int i = 0; i < users.Length; i++)
        {
            string[] userData = users[i].Split(',');//Разбивает текущую строку на части по запятой: userData[0] — логинuserData[1] — парольuserData[2] — роль(модуль)
            if (userData[0] == login)
            {
                userFound = true;//Если логин найден — устанавливаем флаг userFound = true.
                Console.SetCursorPosition(4, 5);
                Console.Write("Введите новый пароль: ");
                string newPassword = ReadPassword();
                Console.SetCursorPosition(4, 6);
                Console.Write("Введите новую роль: ");
                string newRole = Console.ReadLine();
                users[i] = login + "," + newPassword + "," + newRole;//обновляем запись о пользователе в массиве users.
                File.WriteAllLines("users.txt", users);//Перезаписываем весь файл users.txt с обновленной информацией.
                DisplayMessage("Данные пользователя успешно обновлены.", ConsoleColor.Green);
                break;
            }
        }
        if (!userFound)
        {
            DisplayMessage("Пользователь не найден.", ConsoleColor.Red);
        }
    }

    // Удаление пользователя
    static void DeleteUser()
    {
        Console.Clear();
        DrawBox("Удаление пользователя", 2, 2, 60, 15);
        Console.SetCursorPosition(4, 4);
        Console.Write("Введите логин пользователя, которого нужно удалить: ");
        string login = Console.ReadLine();
        string[] users = File.ReadAllLines("users.txt");//Считывает все строки из файла users.txt в массив строк users. Каждая строка имеет формат: login,password,module
        bool userFound = false;
        string[] updatedUsers = new string[users.Length - 1];//Создаём новый массив строк updatedUsers, в котором будет на один элемент меньше , чем в исходном.
                                                             //Это нужно, чтобы удалить одного пользователя.
        int index = 0;//Вспомогательная переменная для индекса в новом массиве updatedUsers.
        for (int i = 0; i < users.Length; i++)
        {
            string[] userData = users[i].Split(',');////Разбивает текущую строку на части по запятой: userData[0] — логинuserData[1] — парольuserData[2] — роль(модуль)
            if (userData[0] == login)//Если логин совпадает с введённым — устанавливаем флаг userFound = true. Эта запись не копируется в новый массив — то есть пользователь "удаляется".
            {
                userFound = true;
            }
            else
            {
                updatedUsers[index] = users[i];
                index++; //Увеличиваем index, чтобы следующий пользователь записался на следующее место.
            }
        }
        if (userFound)
        {
            File.WriteAllLines("users.txt", updatedUsers);
            DisplayMessage("Пользователь успешно удален.", ConsoleColor.Green);
        }
        else
        {
            DisplayMessage("Пользователь не найден.", ConsoleColor.Red);
        }
    }

    // Модуль кассира
    static void CashierModule()
    {
        while (true)
        {
            Console.Clear();
            DrawBox("Модуль 'Кассир'", 2, 2, 60, 15);
            Console.SetCursorPosition(4, 4);
            string[] cashierMenuOptions = {
                "Выбрать фильм",
                "Выбрать дату и сеанс",
                "Выбрать места",
                "Рассчитать стоимость",
                "Формировать билет",
                "Вернуться в главное меню"
            };
            int currentSelection = 0;
            int menuLength = cashierMenuOptions.Length;
            while (true)
            {
                for (int i = 0; i < menuLength; i++)
                {
                    Console.SetCursorPosition(4, 4 + i);
                    if (i == currentSelection)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(cashierMenuOptions[i]);
                    Console.ResetColor();
                }
                Console.SetCursorPosition(4, 4 + menuLength + 1);
                Console.Write("Выберите действие: ");
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                {
                    currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
                }
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentSelection)
                    {
                        case 0:
                            SelectMovie();
                            break;
                        case 1:
                            SelectSession();
                            break;
                        case 2:
                            SelectSeats();
                            break;
                        case 3:
                            CalculateCost();
                            break;
                        case 4:
                            GenerateTicket();
                            break;
                        case 5:
                            return;
                    }
                    break;
                }
            }
        }
    }

    // Выбор фильма
    static void SelectMovie()
    {
        Console.Clear();
        DrawBox("Выбор фильма", 2, 2, 60, 15);
        string[] movies = File.ReadAllLines("movies.txt");//читывает все строки из файла movies.txt в массив строк movies.Каждая строка представляет название одного фильма.

        if (movies.Length == 0)
        {
            DisplayMessage("Нет доступных фильмов.", ConsoleColor.Yellow);
            return;
        }
        int currentSelection = 0;//currentSelection — индекс текущего выбранного фильма. menuLength — количество фильмов в списке.
        int menuLength = movies.Length;
        while (true)
        {
            for (int i = 0; i < menuLength; i++)//Цикл рисует список фильмов.
            {
                Console.SetCursorPosition(4, 4 + i);
                if (i == currentSelection)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine((i + 1) + ". " + movies[i]);
                Console.ResetColor();
            }
            Console.SetCursorPosition(4, 4 + menuLength + 1);
            Console.Write("Выберите фильм (стрелками вверх/вниз, Enter для выбора): ");
            ConsoleKey key = Console.ReadKey(true).Key;//Считывает нажатую клавишу без отображения символа на экране.
            if (key == ConsoleKey.UpArrow)
            {
                currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
            }
            else if (key == ConsoleKey.Enter)//Если нажали Enter — начинаем обработку выбора фильма.
            {
                selectedMovie = movies[currentSelection];
                DisplayMessage("Выбран фильм: " + selectedMovie, ConsoleColor.Green);
                break;
            }
        }
    }

    // Выбор сеанса
    static void SelectSession()
    {
        Console.Clear();
        DrawBox("Выбор сеанса", 2, 2, 60, 15);
        string[] sessions = File.ReadAllLines("sessions.txt");
        if (sessions.Length == 0)
        {
            DisplayMessage("Нет доступных сеансов.", ConsoleColor.Yellow);
            return;
        }
        int currentSelection = 0;
        int menuLength = sessions.Length;
        while (true)
        {
            for (int i = 0; i < menuLength; i++)
            {
                Console.SetCursorPosition(4, 4 + i);
                if (i == currentSelection)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine((i + 1) + ". " + sessions[i]);
                Console.ResetColor();
            }
            Console.SetCursorPosition(4, 4 + menuLength + 1);
            Console.Write("Выберите сеанс (стрелками вверх/вниз, Enter для выбора): ");
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow)
            {
                currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selectedSession = sessions[currentSelection];
                DisplayMessage("Выбран сеанс: " + selectedSession, ConsoleColor.Green);
                break;
            }
        }
    }

    // Выбор места
    static void SelectSeats()
    {
        Console.Clear();
        DrawBox("Выбор мест", 2, 2, 60, 15);
        string[] seats = File.ReadAllLines("seats.txt");
        if (seats.Length == 0)
        {
            DisplayMessage("Нет доступных мест.", ConsoleColor.Yellow);
            return;
        }
        int currentSelection = 0;
        int menuLength = seats.Length;
        while (true)
        {
            for (int i = 0; i < menuLength; i++)
            {
                Console.SetCursorPosition(4, 4 + i);
                if (i == currentSelection)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine((i + 1) + ". " + seats[i]);
                Console.ResetColor();
            }
            Console.SetCursorPosition(4, 4 + menuLength + 1);
            Console.Write("Выберите место (стрелками вверх/вниз, Enter для выбора): ");
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow)
            {
                currentSelection = (currentSelection == 0) ? menuLength - 1 : currentSelection - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentSelection = (currentSelection == menuLength - 1) ? 0 : currentSelection + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selectedSeat = seats[currentSelection];
                DisplayMessage("Выбрано место: " + selectedSeat, ConsoleColor.Green);
                break;
            }
        }
    }

    // Расчёт стоимости
    static void CalculateCost()
    {
        Console.Clear();
        DrawBox("Расчет стоимости", 2, 2, 60, 15);
        Console.SetCursorPosition(4, 4);
        Console.Write("Введите цену билета: ");
        string priceInput = Console.ReadLine();//Считывает введённое значение цены как строку.
        decimal price;//Объявляем переменную price типа decimal для хранения цены билета. decimal используется для точных вычислений с деньгами
        if (decimal.TryParse(priceInput, out price))
        {
            Console.SetCursorPosition(4, 5);
            Console.Write("Введите количество билетов: ");
            string quantityInput = Console.ReadLine();
            int quantity;//Объявляем переменную quantity типа int для хранения количества билетов.
            if (int.TryParse(quantityInput, out quantity) && quantity > 0)//Пытаемся преобразовать строку в целое число.Дополнительно проверяем, чтобы количество было больше нуля.
            {
                ticketPrice = price;
                ticketQuantity = quantity;
                decimal totalCost = price * quantity;
                DisplayMessage("Общая стоимость: " + totalCost.ToString("C"), ConsoleColor.Green);//totalCost.ToString("C") — автоматически применяет валютный формат
                                                                                                  //(например, ₽250,00 или $250.00 в зависимости от региональных настроек).
            }
            else
            {
                DisplayMessage("Некорректное количество билетов.", ConsoleColor.Red);
            }
        }
        else
        {
            DisplayMessage("Некорректная цена.", ConsoleColor.Red);
        }
    }

    // Формирование билета
    static void GenerateTicket()
    {
        Console.Clear();
        DrawBox("Генерация билета", 2, 2, 60, 15);
//Проверяем, были ли выбраны:Фильм(selectedMovie)
//Сеанс(selectedSession)
//Место(selectedSeat)

        if (string.IsNullOrEmpty(selectedMovie) ||
            string.IsNullOrEmpty(selectedSession) ||
            string.IsNullOrEmpty(selectedSeat) ||
            ticketPrice <= 0 || ticketQuantity <= 0)//Цена(ticketPrice) > 0Количество(ticketQuantity) > 0Если каких-то данных нет — выводим ошибку и выходим из метода.
        {
            DisplayMessage("Не все данные выбраны!", ConsoleColor.Red);
            return;
        }

        decimal totalCost = ticketPrice * ticketQuantity;
        string dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");//Получаем текущую дату и время и форматируем её как день.месяц.год часы:минуты:секунды.

        string ticketText = $"Фильм: {selectedMovie}, Сеанс: {selectedSession}, Место: {selectedSeat}, Цена: {ticketPrice:N2} ₽, Количество: {ticketQuantity}, Общая стоимость: {totalCost:N2} ₽, Дата: {dateTime}";
        //ticketPrice:N2 — форматирует число с двумя знаками после запятой (например, 250,50).
        // Показываем билет
        int line = 4;
        foreach (var part in ticketText.Split(','))//Разбиваем строку билета по запятым и выводим каждую часть отдельно.
        {
            Console.SetCursorPosition(4, line++);//line++ — увеличивает номер строки при каждом выводе.
            Console.WriteLine(part.Trim());//Trim() убирает лишние пробелы перед частями.
        }

        // Записываем в файл
        File.AppendAllLines("tickets.txt", new[] { ticketText });//Используется AppendAllLines, чтобы не перезаписывать старые билеты , а добавлять новый в конец файла.

        Console.SetCursorPosition(4, line + 1);
        Console.WriteLine("Билет успешно сохранён!");
        Console.WriteLine("Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    // Модуль афиша
    static void PosterModule()
    {
        Console.Clear();
        DrawBox("Афиша", 2, 2, 60, 15);

        string filePath = "movies.txt";

        if (!File.Exists(filePath))
        {
            DisplayMessage("Файл 'movies.txt' не найден.", ConsoleColor.Red);
            return;
        }

        string[] movies = File.ReadAllLines(filePath);

        if (movies.Length == 0)
        {
            DisplayMessage("Нет доступных фильмов.", ConsoleColor.Yellow);
            return;
        }

        for (int i = 0; i < movies.Length; i++)
        {
            Console.SetCursorPosition(4, 4 + i);
            Console.WriteLine((i + 1) + ". " + movies[i]);
        }

        Console.SetCursorPosition(4, 4 + movies.Length + 1);
        Console.WriteLine("Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }
}
using System.Collections.Concurrent;

namespace Librarian
{
    internal class Program
    {
        private static ConcurrentDictionary<string, int> books = new ConcurrentDictionary<string, int>();

        static void Main(string[] args)
        {
            Thread updateReadingProgressThread = new Thread(UpdateReadingProgress);
            updateReadingProgressThread.IsBackground = true;
            updateReadingProgressThread.Start();

            bool isRunning = true;

            while (isRunning)
            {

                Console.WriteLine("1 - добавить книгу; 2 - вывести список; 3 - выйти");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        ShowBooks();
                        break;
                    case "3":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }
            }

            Console.WriteLine("Программа завершена");
        }

        static void AddBook()
        {
            Console.Write("Введите название книги: ");
            var title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Название не может быть пустым");
                return;
            }

            if (books.TryAdd(title, 0))
                Console.WriteLine($"Книга '{title}' добавлена");
            else
                Console.WriteLine($"Книга '{title}' уже существует");
        }

        static void ShowBooks()
        {
            if (books.IsEmpty)
            {
                Console.WriteLine("Список книг пуст");
                return;
            }

            foreach (var book in books)
                Console.WriteLine($"{book.Key} - {book.Value}%");
        }

        static void UpdateReadingProgress()
        {
            while (true)
            {
                Thread.Sleep(1000);
                {
                    foreach (var book in books)
                    {
                        int newProgress = book.Value + 1;
                        if (newProgress >= 100)
                            books.TryRemove(book.Key, out _);
                        else
                            books.TryUpdate(book.Key, newProgress, book.Value);
                    }
                }
            }
        }
    }
}

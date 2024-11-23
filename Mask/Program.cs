using System;
using System.IO;
using System.Linq;

namespace Mask
{
    class FileSearch
    {
        // Метод для поиска файлов по маске и дате
        public static void SearchFiles(string directory, string mask, DateTime startDate, DateTime endDate, string reportFile)
        {
            try
            {
                // Получаем все файлы по маске и в подкаталогах
                var files = Directory.EnumerateFiles(directory, mask, SearchOption.AllDirectories)
                                     .Where(file => File.GetLastWriteTime(file) >= startDate && File.GetLastWriteTime(file) <= endDate)
                                     .ToList();

                // Записываем результаты в файл отчета
                using (StreamWriter writer = new StreamWriter(reportFile))
                {
                    writer.WriteLine($"Поиск файлов по маске: {mask}");
                    writer.WriteLine($"Дата начала: {startDate:yyyy-MM-dd}");
                    writer.WriteLine($"Дата окончания: {endDate:yyyy-MM-dd}");
                    writer.WriteLine("===========================================");
                    foreach (var file in files)
                    {
                        writer.WriteLine($"Файл: {file} | Дата последней модификации: {File.GetLastWriteTime(file):yyyy-MM-dd HH:mm:ss}");
                    }
                }

                Console.WriteLine($"Результаты поиска записаны в файл: {reportFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Метод для поиска файлов и каталогов по маске по всему диску
        public static void SearchAndPerformAction(string searchPath, string mask)
        {
            try
            {
                var files = Directory.EnumerateFileSystemEntries(searchPath, mask, SearchOption.AllDirectories)
                                     .ToList();

                // Выводим найденные файлы и каталоги
                Console.WriteLine("Найденные файлы и каталоги:");
                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {files[i]}");
                }

                // Запрос действий у пользователя
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Удалить все найденные файлы");
                Console.WriteLine("2 - Удалить указанный файл/каталог");
                Console.WriteLine("3 - Удалить диапазон файлов/каталогов");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        foreach (var file in files)
                        {
                            DeleteFileOrDirectory(file);
                        }
                        Console.WriteLine("Все файлы и каталоги удалены.");
                        break;
                    case 2:
                        Console.WriteLine("Введите номер файла/каталога для удаления:");
                        int index = Convert.ToInt32(Console.ReadLine()) - 1;
                        if (index >= 0 && index < files.Count)
                        {
                            DeleteFileOrDirectory(files[index]);
                            Console.WriteLine($"Файл/каталог {files[index]} удален.");
                        }
                        else
                        {
                            Console.WriteLine("Неверный номер.");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Введите диапазон номеров файлов (например, 2-5):");
                        string range = Console.ReadLine();
                        var ranges = range.Split('-');
                        if (ranges.Length == 2 && int.TryParse(ranges[0], out int start) && int.TryParse(ranges[1], out int end))
                        {
                            for (int i = start - 1; i < end; i++)
                            {
                                if (i >= 0 && i < files.Count)
                                {
                                    DeleteFileOrDirectory(files[i]);
                                    Console.WriteLine($"Файл/каталог {files[i]} удален.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат диапазона.");
                        }
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Метод для удаления файлов или каталогов
        private static void DeleteFileOrDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось удалить {path}: {ex.Message}");
            }
        }

        // Основной метод для запуска приложения
        static void Main(string[] args)
        {
            Console.WriteLine("Поиск файлов по маске и диапазону дат");

            // Задать путь, маску и диапазон дат
            Console.WriteLine("Введите путь к каталогу:");
            string directory = Console.ReadLine();
            Console.WriteLine("Введите маску (например, *.txt):");
            string mask = Console.ReadLine();
            Console.WriteLine("Введите начальную дату (формат: yyyy-MM-dd):");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Введите конечную дату (формат: yyyy-MM-dd):");
            DateTime endDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Введите имя файла отчета:");
            string reportFile = Console.ReadLine();

            // Поиск файлов с сохранением в файл
            SearchFiles(directory, mask, startDate, endDate, reportFile);

            // Поиск файлов по всему диску
            Console.WriteLine("\nВведите путь для поиска по всему диску:");
            string searchPath = Console.ReadLine();
            SearchAndPerformAction(searchPath, mask);
        }
    }
}

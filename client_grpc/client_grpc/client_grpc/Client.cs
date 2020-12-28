using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static contracts.ClientContracts;

namespace grpc_client
{
    public class Client
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск gRPC клиента...\nПопытка подключения к gRPC серверу...");
            //проверка работоспособности сервера
            if (await TestConnection(true) == "Unable to connect gRPC server...")
            {
                Console.WriteLine(await TestConnection(true));
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine(await TestConnection(true));
            }
            bool exitFlag = false;
            while (exitFlag != true)
            {
                Console.WriteLine("Можно использовать две команды: Insert и Read.\nДля завершения работы введите \'q\' или \'exit\'.");
                string cmd = Console.ReadLine();
                if (cmd.ToLower() == "insert")
                {
                    bool isCompleted = false;
                    while (isCompleted == false)
                    {
                        Console.WriteLine("Данный контракт требует три аргумента: string FirstName, string LastName и int Age");
                        Console.WriteLine("Введите имя:");
                        string firstName = Console.ReadLine();
                        Console.WriteLine("Введите фамилию:");
                        string lastName = Console.ReadLine();
                        Console.WriteLine("Введите возраст:");
                        bool parseAge = Int32.TryParse(Console.ReadLine(), out int age);
                        if (parseAge == false)
                        {
                            Console.WriteLine("Не удалось преобразовать возраст к Int32");
                            continue;
                        }
                        else
                        {
                            Validation validation = new Validation(firstName, lastName, age);
                            if (validation.RunValidationInsert() == false)
                            {
                                Console.WriteLine("Валидация не пройдена");
                                continue;
                            }
                            else
                            {
                                InsertContract insertContract = new InsertContract(firstName, lastName, age);
                                Console.WriteLine(await insertContract.InsertRPCAsync());
                                isCompleted = true;
                            }
                        }
                    }
                    
                }
                else if (cmd.ToLower() == "read")
                {
                    bool isCompleted = false;
                    while (isCompleted == false)
                    {
                        Console.WriteLine("Введите Id для поиска:");
                        bool parseId = Int32.TryParse(Console.ReadLine(), out int id);
                        if (parseId == false)
                        {
                            Console.WriteLine("Не удалось преобразовать Id к Int32");
                            continue;
                        }
                        else
                        {
                            Validation validation = new Validation(id);
                            if (validation.RunValidationRead() == false)
                            {
                                Console.WriteLine("Валидация не пройдена");
                                continue;
                            }
                            else
                            {
                                ReadContract readContract = new ReadContract(id);
                                Console.WriteLine(await readContract.ReadRPCAsync());
                                isCompleted = true;
                            }
                            
                        }
                    }
                }
                else if (cmd.ToLower() == "exit" || cmd.ToLower() == "q")
                {
                    await TestConnection(false);
                    Console.WriteLine("Завершение работы сервера и клиента...");
                    exitFlag = true;
                }
                else
                {
                    Console.WriteLine("Команда не распознана, попробуйте еще раз");
                }

            }
            Environment.Exit(0);
        }
        public class Validation
        {
            private static string errors;
            private static bool firstNameIsValid;
            private static bool lastNameIsValid;
            private static bool ageIsValid;
            private static string FirstName;
            private static string LastName;
            private static int Age;
            private static int Id;

            public Validation(string firstName, string lastName, int age)
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
            }
            public Validation(int id)
            {
               Id = id;
            }
            private static bool checkName(string Name)
            {
                if(Name.Length >= 51 || string.IsNullOrWhiteSpace(Name) == true)
                {
                    errors += "Нарушена допустимая длина имени или фамилии в 50 символов, либо поле - пустое\n";
                    return false;
                }
                var expression = new Regex("^.*[^А-Яа-яЁёA-Za-z-]");
                if (expression.IsMatch(Name)==true)
                {
                    errors += "Имя или фамилия содержит спецсимволы\n";
                    return false;
                }
                else
                {
                    errors += "Проверка пройдена\n";
                    return true;
                }
            }
            private static bool checkAge(int Age)
            {
                if (Age.ToString().Length > 3 || string.IsNullOrWhiteSpace(Age.ToString()) == true)
                {
                    errors += "Превышена допустимая длина возраста в 3 символа, или поле возраста - пустое\n";
                    return false;
                }
                var expression = new Regex("^.*[0-9]");
                if (expression.IsMatch(Age.ToString()) != true)
                {
                    errors += "Возраст содержит недопустимые символы\n";
                    return false;
                }
                if (Age < 0)
                {
                    errors += "Возраст не может быть отрицательным\n";
                    return false;
                }
                if (Age > 150)
                {
                    errors += "Возраст выше предела в 150 лет\n";
                    return false;
                }
                else
                {
                    errors += "Возраст прошел проверку\n";
                    return true;
                }
            }
            private static bool checkId(int Id)
            {
                if (Id.ToString().Length > 10 || string.IsNullOrWhiteSpace(Id.ToString()) == true)
                {
                    errors += "Превышена допустимая длина Id в 10 символов\n";
                    return false;
                }
                var expression = new Regex("^.*[^0-9]$");
                if (expression.IsMatch(Id.ToString()) == true)
                {
                    errors += "Id содержит недопустимые символы\n";
                    return false;
                }
                if (Id <= 0)
                {
                    errors += "Id не может быть нулем или отрицательным числом\n";
                    return false;
                }
                else
                {
                    errors += "Id прошел проверку\n";
                    return true;
                }
            }
            public bool RunValidationInsert()
            {
                firstNameIsValid = checkName(FirstName);
                lastNameIsValid = checkName(LastName);
                ageIsValid = checkAge(Age);
                if (firstNameIsValid==false || lastNameIsValid==false || ageIsValid == false)
                {
                    Console.WriteLine(errors);
                    errors = "";
                    return false;
                }
                else
                {
                    Console.WriteLine("Все элементы прошли проверку");
                    return true;
                }
            }
            public bool RunValidationRead()
            {
                if (checkId(Id) == false)
                {
                    Console.WriteLine(errors);
                    errors = "";
                    return false;
                }
                else
                {
                    Console.WriteLine("Id прошел проверку");
                    return true;
                }
            }
        }
        internal class InsertContract
        {
            readonly string FirstName;
            readonly string LastName;
            readonly int Age;
            public InsertContract(string firstName, string lastName, int age)
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
            }
            public async Task<string> InsertRPCAsync()
            {
                return await AddingPersons(FirstName, LastName, Age);
            }
            
        }
        internal class ReadContract
        {
            readonly int Id;
            public ReadContract(int id)
            {
                Id = id;
            }
            public async Task<string> ReadRPCAsync()
            {
                return await PersonsInfo(Id);
            }
        }
    }
}

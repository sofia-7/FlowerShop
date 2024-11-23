using System.Security.Cryptography; 
using System.Text; 

namespace MvcFlowers.Models 
{
    public class User 
    {
        public string Login { get; set; }

        // Приватное поле для хранения пароля в виде массива байтов
        private byte[] password;

        // Свойство для установки и получения пароля
        public string Password
        {
            // Геттер: возвращает хэш пароля в шестнадцатеричном формате
            get
            {
                var sb = new StringBuilder(); // Создаем объект StringBuilder для построения строки
                // Вычисляем хэш пароля с использованием MD5 и добавляем его в StringBuilder
                foreach (var b in new MD5CryptoServiceProvider().ComputeHash(password))
                    sb.Append(b.ToString("x2")); // Преобразуем каждый байт в шестнадцатеричное представление
                return sb.ToString(); // Возвращаем строку с хэшированным паролем
            }
            // Сеттер: принимает строку пароля и преобразует ее в массив байтов
            set { password = Encoding.UTF8.GetBytes(value); } // Кодируем пароль в UTF-8
        }

        // Свойство, которое определяет, является ли пользователь администратором
        public bool IsAdmin => Login == "admin"; // Возвращает true, если логин равен "admin"

        // Метод для проверки правильности введенного пароля
        public bool CheckPassword(string password) => password == Password; // Сравнивает введенный пароль с хэшированным паролем
    }
}

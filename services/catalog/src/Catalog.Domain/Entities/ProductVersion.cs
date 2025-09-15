using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class ProductVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; } // Ссылка на продукт
        public string Sku { get; set; } = default!; // Уникальный артикул версии
        public decimal Price { get; set; }
        public string? Metadata { get; set; } // Доп. параметры (цвет, размер и т.д.)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационное свойство к продукту
        public Product? Product { get; set; }


    }
}

//IEnumerable - только чтение(можно перебирать)

//ICollection - чтение + добавление / удаление

//List - конкретная реализация со всеми методами

//Зачем абстракция? Чтобы можно было легко поменять реализацию:

//Сегодня используем List<T>

//Завтра можем перейти на HashSet<T> для уникальности

//Послезавтра на массив для производительности



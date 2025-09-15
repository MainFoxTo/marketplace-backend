using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Навигационное свойство для версий
        public ICollection<ProductVersion> Versions { get; set; } = new List<ProductVersion>();

    }
}


//Id — уникальный идентификатор (генерируется автоматически).

//Sku — артикул товара.

//Name — название товара.

//Description — описание (может быть null).

//CreatedAt и UpdatedAt — даты создания и обновления.

//Важно: Это чистый C# класс без каких-либо зависимостей от базы данных.


//IEnumerable - только чтение(можно перебирать)

//ICollection - чтение + добавление / удаление

//List - конкретная реализация со всеми методами

//Зачем абстракция? Чтобы можно было легко поменять реализацию:

//Сегодня используем List<T>

//Завтра можем перейти на HashSet<T> для уникальности

//Послезавтра на массив для производительности


using Catalog.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);

        // Versions
        Task AddVersionAsync(ProductVersion version);
        Task<IEnumerable<ProductVersion>> GetVersionsAsync(Guid productId);
    }

}
//IEnumerable - только чтение(можно перебирать)

//ICollection - чтение + добавление / удаление

//List - конкретная реализация со всеми методами

//Зачем абстракция? Чтобы можно было легко поменять реализацию:

//Сегодня используем List<T>

//Завтра можем перейти на HashSet<T> для уникальности

//Послезавтра на массив для производительности




//Определяет контракт для работы с продуктами:

//GetByIdAsync — получить продукт по ID.

//AddAsync — добавить новый продукт.

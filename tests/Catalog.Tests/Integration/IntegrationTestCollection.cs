using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Tests.Integration
{
    // Этот атрибут группирует все интеграционные тесты в одну коллекцию
    [CollectionDefinition("IntegrationTests")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
    {
        // Класс должен быть пустым - это просто маркер
    }
}

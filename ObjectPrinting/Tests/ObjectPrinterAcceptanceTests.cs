using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void CreatePerson()
        {
            person = new Person {Name = "Alex", Age = 19};
        }

        [Test]
        public void ExcludingType_ShouldBeSerializeWithoutThisType()
        {
            var expected = "Person\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = 19\r\n";
            person.PrintToString(s => s.Excluding<Guid>()).Should().Be(expected);

        }

        [Test]
        public void Alternative_ShouldBeSerialize()
        {
            var expected = "Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = 13\r\n";
            person.PrintToString(s => s
                    .Printing<int>()
                    .Using(p => p.ToString("X")))
                .Should().Be(expected);
        }

        [Test]
        public void CultureForNumbers()
        {
            person.Height = 180.5;
            var expected = "Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 180.5\r\n\tAge = 19\r\n";
            person.PrintToString(s => s
                    .Printing<double>()
                    .Using(CultureInfo.InvariantCulture))
                .Should().Be(expected);
            person.Height = 0;
        }

        [Test]
        public void SerializeForProperty()
        {
            var expected = "Person\r\n\tId = Guid\r\n\tName = Alex this is name\r\n\tHeight = 0\r\n\tAge = 19\r\n";
            person.PrintToString(s => s
                    .Printing(p => p.Name)
                    .Using(n => n + " this is name"))
                .Should().Be(expected);
        }

        [Test]
        public void TrimmedString_ShouldBeTrimmed()
        {
            person.Name = "Very long name very long name";
            var expected = "Person\r\n\tId = Guid\r\n\tName = Very long name\r\n\tHeight = 0\r\n\tAge = 19\r\n";
            person.PrintToString(s => s
                    .Printing(p => p.Name)
                    .TrimmedString(14))
                .Should().Be(expected);
            person.Name = "Alex";
        }

        [Test]
        public void ExcludingProperty_ShouldBeSerializeWithoutThisProperty()
        {
            var expected = "Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tAge = 19\r\n";
            person.PrintToString(s => s.Excluding(p => p.Height)).Should().Be(expected);
        }

        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(p => p.ToString("X"))
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => person.Age).Using(a => a.ToString() + "this is age")
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).TrimmedString(3)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Age);

            string s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
            string s2 = person.PrintToString();

            //8. ...с конфигурированием
            string s3 = person.PrintToString(s => s.Excluding(p => p.Age));
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }
    }
}
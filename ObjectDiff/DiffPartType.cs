using System.ComponentModel;

namespace ObjectDiff
{
    /// <summary>
    /// Тип разности
    /// </summary>
    public enum DiffPartType
    {
        /// <summary>
        /// Требуется создание текущего объекта
        /// </summary>
        [Description("Создание нового значения")]
        Create,
        /// <summary>
        /// Требуется удаление текущего объекта
        /// </summary>
        [Description("Удаление текщего значения")]
        Remove,
        /// <summary>
        /// Требуется добавление элемента в текущий объект или изменение элемента
        /// </summary>
        [Description("Добавление в объект значение (для массивов) или изменение текущего значения")]
        Include,
        /// <summary>
        /// Требуется исключение элемента из текущего объекта
        /// </summary>
        [Description("Удаление элемента из текущего объекта (для массивов)")]
        Exclude,
        /// <summary>
        /// Требуется изменение размера (для массивов и индексируемых коллекций)
        /// </summary>
        [Description("Изменение размера (для массивов)")]
        Resize
    }
}

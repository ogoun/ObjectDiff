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
        Create,
        /// <summary>
        /// Требуется удаление текущего объекта
        /// </summary>
        Remove,
        /// <summary>
        /// Требуется добавление элемента в текущий объект или изменение элемента
        /// </summary>
        Include,
        /// <summary>
        /// Требуется исключение элемента из текущего объекта
        /// </summary>
        Exclude,
        /// <summary>
        /// Требуется изменение размера (для массивов и индексируемых коллекций)
        /// </summary>
        Resize
    }
}

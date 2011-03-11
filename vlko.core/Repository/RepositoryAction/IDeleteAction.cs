﻿namespace vlko.core.Repository.RepositoryAction
{
    /// <summary>
    /// Delete action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IDeleteAction<T> : IAction<T> where T : class
    {
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(T item);
    }
}

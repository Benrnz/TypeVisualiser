namespace TypeVisualiser
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an extension to the standard dictionary and is generic usage of the suffix dictionary is ok here.")]
    public interface IReadOnlyDictionary<TKey, TValue>
    {
        bool ContainsKey(TKey key);

        ICollection<TKey> Keys { get; }
        
        ICollection<TValue> Values { get; }
        
        int Count { get; }
        
        bool IsReadOnly { get; }
        
        bool TryGetValue(TKey key, out TValue value);
        
        TValue this[TKey key] { get; }
        
        bool Contains(KeyValuePair<TKey, TValue> item);
        
        void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Standard IDictionary implementation")]
        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
    }
}
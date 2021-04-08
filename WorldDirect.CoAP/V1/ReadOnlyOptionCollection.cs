namespace WorldDirect.CoAP.V1
{
    using System.Collections;
    using System.Collections.Generic;
    using Options;

    /// <summary>
    /// Represents a readonly <see cref="OptionCollection"/> that holds a set of <see cref="CoapOption"/>.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{WorldDirect.CoAP.V1.Options.CoapOption}" />
    public class ReadOnlyOptionCollection : IReadOnlyCollection<CoapOption>
    {
        private readonly OptionCollection collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyOptionCollection"/> class.
        /// </summary>
        /// <param name="values">The set of <see cref="CoapOption"/> that should be inserted.</param>
        public ReadOnlyOptionCollection(IEnumerable<CoapOption> values)
            : this(new OptionCollection(values))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyOptionCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ReadOnlyOptionCollection(OptionCollection collection)
        {
            this.Count = collection.Count;
            this.collection = collection;
        }

        /// <inheritdoc />
        public int Count { get; }

        /// <inheritdoc />
        public IEnumerator<CoapOption> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
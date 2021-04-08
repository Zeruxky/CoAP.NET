namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Codes.Common;
    using Options;

    /// <summary>
    /// Represents a collection of <see cref="CoapOption"/>s.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.ICollection{WorldDirect.CoAP.V1.Options.CoapOption}" />
    public class OptionCollection : ICollection<CoapOption>
    {
        private readonly SortedDictionary<ushort, List<CoapOption>> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionCollection"/> class.
        /// </summary>
        public OptionCollection()
            : this(Enumerable.Empty<CoapOption>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionCollection"/> class.
        /// </summary>
        /// <param name="options">The set of <see cref="CoapOption"/>s for the current <see cref="OptionCollection"/>.</param>
        public OptionCollection(IEnumerable<CoapOption> options)
        {
            var dictionary = options
                .GroupBy(o => o.Number)
                .ToDictionary(o => o.Key, o => o.ToList());
            this.values = new SortedDictionary<ushort, List<CoapOption>>(dictionary);
        }

        /// <inheritdoc />
        public int Count => this.values.Values.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the <see cref="IEnumerable{CoapOption}"/> with the specified number.
        /// </summary>
        /// <value>
        /// The <see cref="IEnumerable{CoapOption}"/>.
        /// </value>
        /// <param name="number">The number of the <see cref="CoapOption"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CoapOption"/>s that are mapped to the specified <paramref name="number"/>.</returns>
        public IEnumerable<CoapOption> this[ushort number]
        {
            get
            {
                return this.values[number];
            }

            set
            {
                this.values[number].AddRange(value);
            }
        }

        /// <summary>
        /// Adds the specified <see cref="CoapOption"/> to the <see cref="OptionCollection"/>.
        /// </summary>
        /// <param name="option">The <see cref="CoapOption"/> to add.</param>
        /// <exception cref="ArgumentNullException">Throws, if the specified <see cref="CoapOption"/> is <see langword="null"/>.</exception>
        public void Add(CoapOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option), "Can not remove a option that is NULL.");
            }

            if (!option.IsRepeatable && this.values.ContainsKey(option.Number))
            {
                option = (UnrecognizedOption)Convert.ChangeType(option, typeof(UnrecognizedOption));
            }

            if (!this.Contains(option))
            {
                this.values.Add(option.Number, new List<CoapOption>());
            }

            this.values[option.Number].Add(option);
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.values.Clear();
        }

        /// <inheritdoc />
        public bool Contains(CoapOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option), "Can not remove a option that is NULL.");
            }

            if (!this.values.ContainsKey(option.Number))
            {
                return false;
            }

            return this.values[option.Number].Contains(option);
        }

        /// <inheritdoc />
        public void CopyTo(CoapOption[] array, int arrayIndex)
        {
            var arraySize = array.Length - arrayIndex;
            if (arraySize < this.values.Values.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The array can not hold all options from the collection.");
            }

            var index = arrayIndex;
            foreach (var option in this.values.SelectMany(options => options.Value))
            {
                array[index] = option;
                index++;
            }
        }

        /// <summary>
        /// Removes the first occurrence of the specified <see cref="CoapOption"/> from the <see cref="OptionCollection" />.
        /// </summary>
        /// <param name="item">The <see cref="CoapOption"/> to remove from the <see cref="OptionCollection" />.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="OptionCollection" />; otherwise, <see langword="false" />.
        ///   This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="OptionCollection" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">Throws, if the provided <see cref="CoapOption"/> is <see langword="null" />.</exception>
        public bool Remove(CoapOption item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Can not remove a option that is NULL.");
            }

            if (!this.values[item.Number].Remove(item))
            {
                return false;
            }

            if (this.values[item.Number].Count == 0)
            {
                return this.values.Remove(item.Number);
            }

            return true;
        }

        /// <inheritdoc />
        bool ICollection<CoapOption>.Remove(CoapOption item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Can not remove a option that is NULL.");
            }

            if (!this.values[item.Number].Remove(item))
            {
                return false;
            }

            if (this.values[item.Number].Count == 0)
            {
                return this.values.Remove(item.Number);
            }

            return true;
        }

        /// <inheritdoc />
        public IEnumerator<CoapOption> GetEnumerator()
        {
            return this.values
                .SelectMany(o => o.Value)
                .GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
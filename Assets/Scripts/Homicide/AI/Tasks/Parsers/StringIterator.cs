using System;
using System.Linq;

namespace Homicide.AI.Tasks.Parsers
{
	public class StringIterator
	{
        private string data;
        private int place;

        public string GetLocationDescription()
        {
            var line = data.Take(place).Count(c => c == '\n');
            var column = 0;
            var p = place - 1;
            while (p >= 0 && data[p] != '\n')
            {
                --p;
                ++column;
            }

            return string.Format("{0}:{1}", line, column);
        }

        public StringIterator(string data)
        {
            this.data = data;
            this.place = 0;
        }

        public StringIterator(string data, int offset)
        {
            this.data = data;
            this.place = offset;
        }

        public string Peek(int count)
        {
            if (place + count > data.Length) throw new IndexOutOfRangeException();
            return data.Substring(place, count);
        }

        public char Next
        {
            get
            {
                if (AtEnd) throw new IndexOutOfRangeException();
                return data[place];
            }
        }

        public bool AtEnd
        {
            get { return place >= data.Length; }
        }

        public StringIterator Advance()
        {
            return new(data, place + 1);
        }

        public StringIterator Advance(int count)
        {
            return new(data, place + count);
        }

        public StringIterator Rewind()
        {
            return new(data, Math.Max(0, place - 1));
        }

        public StringIterator Rewind(int count)
        {
            return new(data, Math.Max(0, place - count));
        }
    }
}

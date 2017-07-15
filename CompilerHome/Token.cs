namespace CompilerHome
{
    public class Token
    {
        private string type;
        private string word;

        public Token(string word, string type)
        {
            this.type = type;
            this.Word = word;
        }

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public string Word
        {
            get
            {
                return word;
            }

            set
            {
                word = value;
            }
        }
    }
}
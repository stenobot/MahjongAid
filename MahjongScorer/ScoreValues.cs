namespace MahjongScorer
{
    public static class ScoreValues
    {
        public const int SELF_DRAWN_SCORE = 2;
        public const int ONE_CHANCE_SCORE = 2;
        public const int LUCKY_PAIR_SCORE = 2;
        public const int CONCEALED_SCORE = 10;

        public const int BASE_PUNG_VALUE = 2;
        public const int BASE_KONG_VALUE = 8;

        // multiply score values
        public const int X_ISWINNER = 4;
        public const int X_ISWINNER_ISDEALER = 6;
        public const int X_ISLOSER_SELFDRAWN = -1;
        public const int X_ISLOSER_ISDEALER_SELFDRAWN = -2;       
        public const int X_ISLOSER_SELFDRAWN_DEALERWON = -2;
        public const int X_ISLOSER_ISDRAWNFROM = -4;
        public const int X_ISLOSER_ISDRAWNFROM_DEALERWON = -6;
    }
}

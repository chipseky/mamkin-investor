namespace Chipseky.MamkinInvestor.Domain;

public static class LotSizeFilterHelper
{
    public static int GetDecimals(decimal basePrecision)
    {
        if (basePrecision == 0)
            return 0;
        
        if (basePrecision >= 1)
            return 0;

        var result = 0;
        while (basePrecision < 1)
        {
            basePrecision *= 10;
            result++;
        }

        return result;
    }
}
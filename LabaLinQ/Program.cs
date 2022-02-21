// See https://aka.ms/new-console-template for more information
public class Program
{
    private static ForecastCollection ReadCsv(string filename)
    {
        List<ForecastEvents> forecastList = new List<ForecastEvents>();
        using (StreamReader sr = new StreamReader(filename))
        {
            sr.ReadLine();
            for (int i = 0; i < 10; i++)
            {
                forecastList.Add(ForecastEvents.GetClassFromCsvLine(sr.ReadLine()));
            }
        }
        return new ForecastCollection(forecastList);
    }

    private static void GetAmountOfEventsIn2018(ForecastCollection forecasts) => Console.WriteLine(forecasts.Where(forecast => forecast.startTime.Year == 2018)
                                                                                                            .Count());

    private static void GetAmountOfStates(ForecastCollection forecasts)
    {
        Console.WriteLine(forecasts.GroupBy(forecast => forecast.State)
                   .Count());
        Console.WriteLine(forecasts.GroupBy(forecast => forecast.City)
                 .Count());
    }

    private static void GetMostRainyCity(ForecastCollection forecasts)
    {
        foreach (var city in forecasts.Where(forecast => forecast.eventType == ForecastsTypes.Rain)
                                      .GroupBy(forecast => forecast.City)
                                      .Select(g => new { City = g.Key, Count = g.Distinct().Count() }).OrderBy(x => -x.Count)
                                      .Take(3))
            Console.WriteLine($"{city.City} | {city.Count}");
    }

    private static void GetTheLongestSnowFalls(ForecastCollection forecasts)
    {
        var snowFalls = forecasts.Where(forecast => forecast.eventType == ForecastsTypes.Snow).GroupBy(forecast => forecast.City);
        foreach (var city in snowFalls)
        {
            foreach (var x in city.GroupBy(forecast => forecast.startTime.Year))
            {
                var res = x.OrderBy(y => y.endTime - y.startTime).Last();
                Console.WriteLine($"{city.Key} | {x.Key} => {res}");
            }

        }
    }

    private static void GetFirstafter2HoursEvent(ForecastCollection forecasts)
    {
        var forecasts2019ByStates = forecasts.Where(x => x.startTime.Year == 2019).GroupBy(x => x.State);
        foreach (var forecast in forecasts2019ByStates)
            Console.WriteLine($"{forecast.Key} | {forecast.SkipWhile(x => (x.endTime - x.startTime).TotalHours >= 2).ElementAt(1)}");
    }

    // private static void GetSumOfHoursInStates(ForecastCollection forecasts)
    // {
    //     var forecastsSevere2017ByStates = forecasts.Where(x => x.startTime.Year == 2017 && x.severity == Severity.Severe).GroupBy(x => x.State).Select(x => new { State = x.Key, city = x.GroupBy(c => c.City) };

    //     foreach (var forecast in forecastsSevere2017ByStates)
    //         Console.WriteLine($"{forecast.Key} | {forecast}");
    // }

    public static void Main(string[] args)
    {
        var ForecastList = ReadCsv("C:\\Users\\user\\Documents\\datasets\\WeatherEvents_Jan2016-Dec2020.csv");
        foreach (var forecast in ForecastList)
        {
            Console.WriteLine(forecast);
        }

        // Task 0
        GetAmountOfEventsIn2018(ForecastList);

        // Task 1
        GetAmountOfStates(ForecastList);

        // Task 2
        GetMostRainyCity(ForecastList);

        // Task 3
        GetTheLongestSnowFalls(ForecastList);

        // Task 4
        GetFirstafter2HoursEvent(ForecastList);
    }
}
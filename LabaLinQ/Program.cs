// See https://aka.ms/new-console-template for more information
public class Program
{
    private static ForecastCollection ReadCsv(string filename)
    {
        List<ForecastEvents> forecastList = new List<ForecastEvents>();
        using (StreamWriter sw = new StreamWriter("InvalidData.log"))
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                string? input = sr.ReadLine();
                int i = 0;
                while (input != null)
                {
                    try
                    {
                        i++;
                        if (i % 1000000 == 0)
                            Console.WriteLine(i / 1000000);

                        input = sr.ReadLine();
                        forecastList.Add(ForecastEvents.GetClassFromCsvLine(input != null ? input : string.Empty));
                    }
                    // catch (ArgumentException e)
                    // {
                    //     sw.WriteLine($"{e.Message} at element {i}");
                    // }
                    // catch (IndexOutOfRangeException)
                    // {
                    //     sw.WriteLine($"Elements in index {i} does not have all parametrs");
                    // }
                    catch (Exception e)
                    {
                        // sw.WriteLine(e.Message);
                    }
                }
            }
        }
        return new ForecastCollection(forecastList);
    }

    private static int GetAmountOfEventsIn2018(ForecastCollection forecasts) => forecasts.Where(forecast => forecast.startTime.Year == 2018)
                                                                                                            .Count();

    private static StateCityAmoutOfEvents GetAmountOfStates(ForecastCollection forecasts)
    {
        int stateAmountOfEvents = forecasts.GroupBy(forecast => forecast.State)
                   .Count();
        int cityAmountOfEvents = forecasts.GroupBy(forecast => forecast.City)
                 .Count();
        return new StateCityAmoutOfEvents(stateAmountOfEvents, cityAmountOfEvents);
    }

    private static List<MostRainyCity> GetMostRainyCity(ForecastCollection forecasts)
    {
        var top3Cities = new List<MostRainyCity>();
        foreach (var city in forecasts.Where(forecast => forecast.eventType == ForecastsTypes.Rain)
                                      .GroupBy(forecast => forecast.City)
                                      .Select(g => new { City = g.Key, Count = g.Distinct().Count() }).OrderByDescending(x => x.Count)
                                      .Take(3))
            top3Cities.Add(new MostRainyCity(city.City, city.Count));
        return top3Cities;
    }

    private static List<BiggestSnowFall> GetTheLongestSnowFalls(ForecastCollection forecasts)
    {
        var snowFallsList = new List<BiggestSnowFall>();
        var snowFalls = forecasts.Where(forecast => forecast.eventType == ForecastsTypes.Snow).GroupBy(forecast => forecast.startTime.Year);
        foreach (var ev in snowFalls)
        {
            var res = ev.OrderByDescending(y => y.endTime - y.startTime).First();
            snowFallsList.Add(new BiggestSnowFall(ev.Key, res.City, res.startTime, res.endTime));
        }

        return snowFallsList;
    }

    private static List<EventAfterBiggest> GetFirstafter2HoursEvent(ForecastCollection forecasts)
    {
        var EventsAfterBiggestList = new List<EventAfterBiggest>();
        var forecasts2019ByStates = forecasts.Where(x => x.startTime.Year == 2019).GroupBy(x => x.State);
        foreach (var forecast in forecasts2019ByStates)
            EventsAfterBiggestList.Add(new EventAfterBiggest(forecast.Key, (forecast.SkipWhile(x => (x.endTime - x.startTime).TotalHours >= 2)).Count()));

        return EventsAfterBiggestList;
    }

    private static List<CityWithMaxDurationOfSevereWeather> GetSumOfHoursInStates(ForecastCollection forecasts)
    {
        var citiesWithDurationOfSevereWeather = new List<CityWithMaxDurationOfSevereWeather>();
        var citiesInStates = forecasts.Where(x => x.severity == Severity.Severe & x.startTime.Year == 2017)
                                        .Select(x => new
                                        {
                                            state = x.State,
                                            city = x.City,
                                            duration = x.endTime.Subtract(x.startTime)
                                        })
                                        .GroupBy(x => x.state)
                                        .Select(x => new
                                        {
                                            state = x.Key,
                                            city = x
                                                    .GroupBy(g => g.city)
                                                    .Select(h => new
                                                    {
                                                        city = h.Key,
                                                        sumDuration = h
                                                            .Select(j => j.duration)
                                                            .Sum(soh => soh.Hours)
                                                    })
                                                    .OrderByDescending(x => x.sumDuration).First()
                                        })
                                        .Select(x => new
                                        {
                                            state = x.state,
                                            city = x.city.city,
                                            maxSumDuration = x.city.sumDuration
                                        })
                                        .OrderByDescending(x => x.maxSumDuration);
        foreach (var cities in citiesInStates)
        {
            citiesWithDurationOfSevereWeather.Add(new CityWithMaxDurationOfSevereWeather(cities.state, cities.city, cities.maxSumDuration));
        }

        return citiesWithDurationOfSevereWeather;
    }

    private static List<PopularEvent> GetMostCommonEvents(ForecastCollection forecasts)
    {
        var mostCommonEvents = new List<PopularEvent>();
        var CommonEvents = forecasts.GroupBy(x => x.startTime.Year).Select(g => new
        {
            Year = g.Key,
            Event = g.GroupBy(e => e.eventType)
                     .Select(x => new { Event = x.Key, Count = x.Count() })
                     .OrderByDescending(p => p.Count)
                     .Select(x => x.Event)
                     .First()
        });

        foreach (var mostCommonEvent in CommonEvents)
            mostCommonEvents.Add(new PopularEvent(mostCommonEvent.Year, mostCommonEvent.Event, forecasts.Where(e => e.startTime.Year == mostCommonEvent.Year && e.eventType == mostCommonEvent.Event).Average(e => e.endTime.Subtract(e.startTime).TotalMinutes)));
        Console.WriteLine("Most rare events");

        return mostCommonEvents;

    }

    private static List<PopularEvent> GetMostRareEvents(ForecastCollection forecasts)
    {

        var mostRareEvents = new List<PopularEvent>();
        var RareEvents = forecasts.GroupBy(x => x.startTime.Year).Select(g => new
        {
            Year = g.Key,
            Event = g.GroupBy(e => e.eventType)
                     .Select(x => new { Event = x.Key, Count = x.Count() })
                     .OrderBy(p => p.Count)
                     .Select(x => x.Event)
                     .First()
        });

        foreach (var mostCommonEvent in RareEvents)
            mostRareEvents.Add(new PopularEvent(mostCommonEvent.Year, mostCommonEvent.Event, forecasts.Where(e => e.startTime.Year == mostCommonEvent.Year && e.eventType == mostCommonEvent.Event).Average(e => e.endTime.Subtract(e.startTime).TotalMinutes)));

        return mostRareEvents;
    }

    public static void Main(string[] args)
    {
        var ForecastList = ReadCsv("C:\\Users\\user\\Documents\\datasets\\WeatherEvents_Jan2016-Dec2020.csv");

        Console.WriteLine("==================Task 0==================");
        // Task 0
        Console.WriteLine(GetAmountOfEventsIn2018(ForecastList));

        Console.WriteLine("==================Task 1==================");
        // Task 1
        Console.WriteLine(GetAmountOfStates(ForecastList));

        Console.WriteLine("==================Task 2==================");
        // Task 2
        foreach (var city in GetMostRainyCity(ForecastList))
            Console.WriteLine(city);

        Console.WriteLine("==================Task 3==================");
        // Task 3
        foreach (var snowfall in GetTheLongestSnowFalls(ForecastList))
            Console.WriteLine(snowfall);

        Console.WriteLine("==================Task 4==================");
        // Task 4
        foreach (var events in GetFirstafter2HoursEvent(ForecastList))
            Console.WriteLine(events);

        Console.WriteLine("==================Task 5==================");
        // Task 5
        foreach (var state in GetSumOfHoursInStates(ForecastList))
        {
            Console.WriteLine(state);
        }

        Console.WriteLine("==================Task 6==================");
        // Task 6
        Console.WriteLine("Most common events");
        foreach (var year in GetMostCommonEvents(ForecastList))
            Console.WriteLine(year);
        Console.WriteLine("Most rare events");
        foreach (var year in GetMostRareEvents(ForecastList))
            Console.WriteLine(year);

    }
}
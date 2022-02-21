record class StateCityAmoutOfEvents(int AmountInState,
                                    int AmountInCity);

record class MostRainyCity(string City, int AmountOfRains);

record class BiggestSnowFall(int Year,
                             string City,
                             DateTime startTime,
                             DateTime endTime);

record class EventAfterBiggest(string State,
                                int AmountOfEvents);

record class CityWithMaxDurationOfSevereWeather(string State,
                                                string City,
                                                int AmountOfHours);

record class PopularEvent(int Year,
                          ForecastsTypes Type,
                          double DurationInMinutes);
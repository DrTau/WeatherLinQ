enum ForecastsTypes { SevereCold, Fog, Hail, Rain, Snow, Storm, Other };
enum Severity { Severe, Moderate, Light, Heavy };

record class ForecastEvents(string eventID,
                      ForecastsTypes eventType,
                      Severity severity,
                      DateTime startTime,
                      DateTime endTime,
                      string TimeZone,
                      string AirportCode,
                      double LocationLat,
                      double LocationLng,
                      string City,
                      string Country,
                      string State,
                      string ZipCode)
{

    public static ForecastEvents GetClassFromCsvLine(string csvString)
    {
        var data = csvString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return new ForecastEvents(data[0],
                            (ForecastsTypes)Enum.Parse(typeof(ForecastsTypes), data[1]),
                            (Severity)Enum.Parse(typeof(Severity), data[2]),
                            ParseDate(data[3]),
                            ParseDate(data[4]),
                            data[5],
                            data[6],
                            double.Parse(data[7].Replace('.', ',')),
                            double.Parse(data[8].Replace('.', ',')),
                            data[9],
                            data[10],
                            data[11],
                            data[12]);
    }

    private static DateTime ParseDate(string dateString)
    {
        string date, time;
        (date, time) = (dateString.Split(' ')[0], dateString.Split(' ')[1]);
        var dateArr = new List<string>(date.Split('-'));
        dateArr.AddRange(time.Split(':'));
        List<int> dateList = dateArr.ConvertAll<int>(int.Parse);
        return new DateTime(dateList[0], dateList[1], dateList[2], dateList[3], dateList[4], dateList[5]);
    }

}
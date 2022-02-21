using System.Collections;

class ForecastCollection : IEnumerable<ForecastEvents>
{
    List<ForecastEvents> forecastList = new List<ForecastEvents>();

    public ForecastCollection()
    {
        this.forecastList = new List<ForecastEvents>();
    }

    public ForecastCollection(List<ForecastEvents> forecastList)
    {
        this.forecastList = new List<ForecastEvents>(forecastList);
    }

    IEnumerator<ForecastEvents> IEnumerable<ForecastEvents>.GetEnumerator() => this.forecastList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.forecastList.GetEnumerator();
}
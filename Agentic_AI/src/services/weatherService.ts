export interface WeatherResult {
  location: string;
  temperature: number;
  windSpeed: number;
  description: string;
}

export async function getWeather(city: string): Promise<WeatherResult> {
  // 1. Geocode the city name to coordinates
  const geoRes = await fetch(
    `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(city)}&count=1`,
  );

  const geoData = (await geoRes.json()) as {
    results?: {
      latitude: number;
      longitude: number;
      name: string;
      country: string;
    }[];
  };

  if (!geoData.results?.length) {
    throw new Error(`Couldn't find this location: ${city}`);
  }

  const { latitude, longitude, name, country } = geoData.results[0]!;

  // 2. fetch the current weather using the coordinates
  const weatherRes = await fetch(
    `https://api.open-meteo.com/v1/forecast?latitude=${latitude}&longitude=${longitude}&current=temperature_2m,wind_speed_10m,weather_code`,
  );

  const weatherData = (await weatherRes.json()) as {
    current: {
      temperature_2m: number;
      wind_speed_10m: number;
      weather_code: number;
    };
  };

  const current = weatherData.current;

  return {
    location: `${name}, ${country}`,
    temperature: current.temperature_2m,
    windSpeed: current.wind_speed_10m,
    description: weatherCodeToDescription(current.weather_code),
  };
}

// https://open-meteo.com/en/docs#weathervariables
function weatherCodeToDescription(code: number): string {
  const codes: Record<number, string> = {
    0: "Clear sky",
    1: "Mainly clear",
    2: "Partly cloudy",
    3: "Overcast",
    45: "Foggy",
    48: "Depositing rime fog",
    51: "Light drizzle",
    53: "Moderate drizzle",
    55: "Dense drizzle",
    61: "Slight rain",
    63: "Moderate rain",
    65: "Heavy rain",
    71: "Slight snowfall",
    73: "Moderate snowfall",
    75: "Heavy snowfall",
    80: "Slight rain showers",
    81: "Moderate rain showers",
    82: "Violent rain showers",
    95: "Thunderstorm",
    96: "Thunderstorm with slight hail",
    99: "Thunderstorm with heavy hail",
  };
  return codes[code] ?? "Unknown";
}

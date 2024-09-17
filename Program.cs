using System.Text;

int sampleRate = 44100;
short bitsPerSample = 16;
short channels = 2;
int duration = 30;


byte[] waveData = GenerateSineWaveData(sampleRate, channels, bitsPerSample, duration, 261.63);


FileStream fs = new("output.wav", FileMode.Create);
try
{
    WriteWavHeader(fs, waveData.Length, sampleRate, channels, bitsPerSample);
    fs.Write(waveData, 0, waveData.Length);
}
finally
{
    fs.Close();
}

Console.WriteLine("Arquivo WAV criado.");

byte[] GenerateSineWaveData(int sampleRate, short channels, short bitsPerSample, int duration, double frequency)
{
    int totalSamples = sampleRate * duration;
    int bytesPerSample = bitsPerSample / 8;
    byte[] data = new byte[totalSamples * bytesPerSample * channels];

    for (int i = 0; i < totalSamples; i++)
    {
        double t = (double)i / sampleRate;
        short sampleValue = (short)(Math.Sin(2 * Math.PI * frequency * t) * short.MaxValue);

        for (int channel = 0; channel < channels; channel++)
        {
            int index = (i * channels + channel) * bytesPerSample;
            data[index] = (byte)(sampleValue & 0xFF);
            data[index + 1] = (byte)((sampleValue >> 8) & 0xFF);
        }
    }

    return data;
}

void WriteWavHeader(Stream stream, int dataLength, int sampleRate, short channels, short bitsPerSample)
{
    int subChunk1Size = 16;
    short audioFormat = 1;
    int byteRate = sampleRate * channels * bitsPerSample / 8;
    short blockAlign = (short)(channels * bitsPerSample / 8);

    BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true); // Deixa o stream aberto

    writer.Write(Encoding.ASCII.GetBytes("RIFF"));
    writer.Write(36 + dataLength);
    writer.Write(Encoding.ASCII.GetBytes("WAVE"));

    writer.Write(Encoding.ASCII.GetBytes("fmt "));
    writer.Write(subChunk1Size);
    writer.Write(audioFormat);
    writer.Write(channels);
    writer.Write(sampleRate);
    writer.Write(byteRate);
    writer.Write(blockAlign);
    writer.Write(bitsPerSample);

    writer.Write(Encoding.ASCII.GetBytes("data"));
    writer.Write(dataLength);

    writer.Flush();
}

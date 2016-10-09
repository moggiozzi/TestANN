using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestANN
{
    class WavHelper
    {
    }
    public class WaveHeader
    {
        public string sGroupID; // RIFF
        public uint dwFileLength; // total file length minus 8, which is taken up by RIFF
        public string sRiffType; // always WAVE

        /// <summary>
        /// Initializes a WaveHeader object with the default values.
        /// </summary>
        public WaveHeader()
        {
            dwFileLength = 0;
            sGroupID = "RIFF";
            sRiffType = "WAVE";
        }
    }

    public class WaveFormatChunk
    {
        public string sChunkID;         // Four bytes: "fmt "
        public uint dwChunkSize;        // Length of header in bytes
        public ushort wFormatTag;       // 1 (MS PCM)
        public ushort wChannels;        // Number of channels
        public uint dwSamplesPerSec;    // Frequency of the audio in Hz... 44100
        public uint dwAvgBytesPerSec;   // for estimating RAM allocation
        public ushort wBlockAlign;      // sample frame size, in bytes
        public ushort wBitsPerSample;    // bits per sample

        /// <summary>
        /// Initializes a format chunk with the following properties:
        /// Sample rate: 44100 Hz
        /// Channels: Stereo
        /// Bit depth: 16-bit
        /// </summary>
        public WaveFormatChunk()
        {
            sChunkID = "fmt ";
            dwChunkSize = 16;
            wFormatTag = 1;
            wChannels = 1;
            dwSamplesPerSec = 16000;// 44100;
            wBitsPerSample = 16;
            wBlockAlign = (ushort)(wChannels * (wBitsPerSample / 8));
            dwAvgBytesPerSec = dwSamplesPerSec * wBlockAlign;
        }
    }

    public class WaveDataChunk
    {
        public string sChunkID;     // "data"
        public uint dwChunkSize;    // Length of header in bytes
        public short[] shortArray;  // 8-bit audio

        /// <summary>
        /// Initializes a new data chunk with default values.
        /// </summary>
        public WaveDataChunk()
        {
            shortArray = new short[0];
            dwChunkSize = 0;
            sChunkID = "data";
        }
    }

    public enum WaveExampleType
    {
        ExampleSineWave = 0
    }

    public class WaveGenerator
    {
        // Header, Format, Data chunks
        WaveHeader header;
        WaveFormatChunk format;
        WaveDataChunk data;
        public short[] getData()
        {
            return data.shortArray;
        }
        public void setData(short[] ndata)
        {
            data.shortArray = ndata;
        }
        public WaveGenerator(WaveExampleType type = WaveExampleType.ExampleSineWave)
        {
            // Init chunks
            header = new WaveHeader();
            format = new WaveFormatChunk();
            data = new WaveDataChunk();

            // Fill the data array with sample data
            switch (type)
            {
                case WaveExampleType.ExampleSineWave:
                    int amplitude = 32760;  // Max amplitude for 16-bit audio
                    double freq = 660.0;

                    // Number of samples = sample rate * channels * bytes per sample
                    //uint numSamples = format.dwSamplesPerSec * format.wChannels;
                    uint numSamples = (uint)(format.dwSamplesPerSec / freq);
                    // Initialize the 16-bit array
                    data.shortArray = new short[numSamples];

                    // The "angle" used in the function, adjusted for the number of channels and sample rate.
                    // This value is like the period of the wave.
                    double t = (Math.PI * 2 * freq) / (format.dwSamplesPerSec * format.wChannels);

                    for (uint i = 0; i < numSamples - 1; i++)
                    {
                        // Fill with a simple sine wave at max amplitude
                        for (int channel = 0; channel < format.wChannels; channel++)
                        {
                            data.shortArray[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                        }
                    }

                    // Calculate data chunk size in bytes
                    data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));

                    break;
            }
        }

        public void Save(string filePath)
        {
            // Create a file (it always overwrites)
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            // Use BinaryWriter to write the bytes to the file
            BinaryWriter writer = new BinaryWriter(fileStream);

            // Write the header
            writer.Write(header.sGroupID.ToCharArray());
            writer.Write(header.dwFileLength);
            writer.Write(header.sRiffType.ToCharArray());

            // Write the format chunk
            writer.Write(format.sChunkID.ToCharArray());
            writer.Write(format.dwChunkSize);
            writer.Write(format.wFormatTag);
            writer.Write(format.wChannels);
            writer.Write(format.dwSamplesPerSec);
            writer.Write(format.dwAvgBytesPerSec);
            writer.Write(format.wBlockAlign);
            writer.Write(format.wBitsPerSample);

            // Write the data chunk
            writer.Write(data.sChunkID.ToCharArray());
            writer.Write(data.dwChunkSize*data.shortArray.Count()*(format.wBitsPerSample/8));
            foreach (short dataPoint in data.shortArray)
            {
                writer.Write(dataPoint);
            }

            writer.Seek(4, SeekOrigin.Begin);
            uint filesize = (uint)writer.BaseStream.Length;
            writer.Write(filesize - 8);

            // Clean up
            writer.Close();
            fileStream.Close();
        }

        public void Read(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fileStream);
            // Read the header
            //writer.Write(header.sGroupID.ToCharArray());
            int riffstr = reader.ReadInt32(); //"RIFF"
            header.dwFileLength = reader.ReadUInt32();
            int wavestr = reader.ReadInt32(); //"WAVE"
            // Read the format chunk
            int fmt_str = reader.ReadInt32(); //"fmt "
            format.dwChunkSize = reader.ReadUInt32();
            format.wFormatTag = reader.ReadUInt16();
            format.wChannels = reader.ReadUInt16();
            format.dwSamplesPerSec = reader.ReadUInt32();
            format.dwAvgBytesPerSec = reader.ReadUInt32();
            format.wBlockAlign = reader.ReadUInt16();
            format.wBitsPerSample = reader.ReadUInt16();
            if (format.wBitsPerSample != 16)
                throw new Exception("Not supported bit depth!");
            if (format.dwChunkSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            //// Write the data chunk
            int datastr = reader.ReadInt32();//"data"
            data.dwChunkSize = reader.ReadUInt32();
            //todo Check real data size
            uint sampleCount = data.dwChunkSize / ((uint)format.wBitsPerSample / 8);
            data.shortArray = new short[sampleCount];
            for (int i=0;i<sampleCount;i++)
            {
                data.shortArray[i] = reader.ReadInt16();
            }
            reader.Close();
            fileStream.Close();

        }
    }
}

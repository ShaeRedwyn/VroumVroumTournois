using System.Collections.Generic;
using System;

[Serializable]
public class NeuralNetwork
{
    public int[] layers;
    public float[][] neurons;
    public float[][][] axones;

    //Nombre de layers de neurones.
    int x;
    //Nombre de neurones dans la colonne.
    int y;
    //nombre d'axones connecté au neurone en question.
    int z;

    public NeuralNetwork()
    { 
    
    }

    public void CopyNet(NeuralNetwork netCopy)
    {
        for (x = 0; x < netCopy.axones.Length; x++)
        {
            for (y = 0; y < netCopy.axones[x].Length; y++)
            {
                for (z = 0; z < netCopy.axones[x][y].Length; z++)
                {
                    axones[x][y][z] = netCopy.axones[x][y][z];
                }
            }
        }
    }

    public NeuralNetwork(int[] _layers)
    {
        layers = new int[_layers.Length];

        for (x = 0; x < _layers.Length; x++)
        {
            layers[x] = _layers[x];
        }

        InitNeurones();
        InitAxones();
    }

    void InitNeurones()
    {
        neurons = new float[layers.Length][];

        for (x = 0; x < layers.Length; x++)
        {
            neurons[x] = new float[layers[x]];
        }
    }

    void InitAxones()
    {
        axones = new float[layers.Length - 1][][];

        for (x = 0; x < axones.Length; x++)
        {
            axones[x] = new float[layers[x + 1]][];

            for (int y = 0; y < axones[x].Length; y++)
            {
                axones[x][y] = new float[layers[x]];

                for (z = 0; z < layers[x]; z++)
                {
                    axones[x][y][z] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    float value;
    public void FeedForward(float[] inputs)
    {
        neurons[0] = inputs;

        for (x = 1; x < layers.Length; x++)
        {
            for (y = 0; y < layers[x]; y++)
            {
                value = 0;

                for (z = 0; z < layers[x - 1]; z++)
                {
                    value += neurons[x - 1][z] * axones[x - 1][y][z];
                }

                neurons[x][y] = (float)Math.Tanh(value);
            }
        }
    }


    float randomNumber;
    public void Mutate(float probability)
    {
        for (x = 0; x < axones.Length; x++)
        {
            for (y = 0; y < axones[x].Length; y++)
            {
                for (z = 0; z < axones[x][y].Length; z++)
                {
                    randomNumber = UnityEngine.Random.Range(0f, 100f);

                    if (randomNumber < 0.06f * probability)
                    {
                        axones[x][y][z] = UnityEngine.Random.Range(-1f, 1f);
                    }
                    else if (randomNumber < 0.07f * probability)
                    {
                        axones[x][y][z] *= -1;
                    }
                    else if (randomNumber < 0.5f * probability)
                    {
                        axones[x][y][z] += 0.1f * UnityEngine.Random.Range(-1f, 1f);
                    }
                    else if (randomNumber < 0.75f * probability)
                    {
                        axones[x][y][z] *= UnityEngine.Random.Range(0, 1f) + 1f;
                    }
                    else if (randomNumber < 1f)
                    {
                        axones[x][y][z] *= UnityEngine.Random.Range(0, 1f);
                    }
                }
            }
        }
    }
}  



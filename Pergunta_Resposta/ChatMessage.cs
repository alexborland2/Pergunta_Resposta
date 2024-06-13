using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pergunta_Resposta
{
    public class ChatMessage
    {

        [LoadColumn(0)]
        public string InputMessage { get; set; }

        [LoadColumn(1)]
        public string ResponseMessage { get; set; }
    }
    public class ChatResponse
    {
        [ColumnName("PredictedLabel")]
        public string ResponseMessage { get; set; }
    }

    public class ChatBot
    {
        private readonly MLContext mlContext;
        private readonly ITransformer model;
        private const string ModelPath = "data/model.zip"; // Caminho para salvar o modelo

        public ChatBot()
        {

            mlContext = new MLContext();
            if (File.Exists(ModelPath))
            {
                model = mlContext.Model.Load(ModelPath, out var _);
            }
            else
            {
                model = TrainModel();
                SaveModel(model);
            }
        }

        private ITransformer TrainModel()
        {
            // Caminho relativo para a pasta de dados
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "chat_data.csv");

            // Verifique se o arquivo existe
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException($"Arquivo não encontrado: {dataPath}");
            }

            var dataView = mlContext.Data.LoadFromTextFile<ChatMessage>(dataPath, hasHeader: true, separatorChar: ';');

            // Verificação de leitura dos dados
            var preview = dataView.Preview();
            if (preview.RowView.Length == 0)
            {
                throw new InvalidOperationException("Training set has 0 instances, aborting training.");
            }

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ChatMessage.ResponseMessage))
                .Append(mlContext.Transforms.Text.FeaturizeText("TextFeatures", nameof(ChatMessage.InputMessage)))
                .Append(mlContext.Transforms.Concatenate("Features", "TextFeatures"))
                .AppendCacheCheckpoint(mlContext); // Adicione um checkpoint para melhorar a performance

            var trainingPipeline = pipeline
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = trainingPipeline.Fit(dataView);
            return model;
        }
        private void SaveModel(ITransformer model)
        {
            mlContext.Model.Save(model, null, ModelPath);
        }
        public string GetResponse(string input)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ChatMessage, ChatResponse>(model);
            var prediction = predictionEngine.Predict(new ChatMessage { InputMessage = input });
            return prediction.ResponseMessage;
        }
    }


}

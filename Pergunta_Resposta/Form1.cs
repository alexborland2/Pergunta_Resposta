using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pergunta_Resposta
{
    public partial class Form1 : Form
    {

        private ChatBot chatBot;
        public Form1()
        {
            InitializeComponent();
            chatBot = new ChatBot();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string userInput = txtInput.Text;
            string botResponse = chatBot.GetResponse(userInput);
            rtbChat.AppendText("Você: " + userInput + "\n");
            rtbChat.AppendText("Bot: " + botResponse + "\n");
            txtInput.Clear();


            /*string userInput = txtInput.Text;
            string botResponse = GetBotResponse(userInput);
            rtbChat.AppendText("Você: " + userInput + "\n");
            rtbChat.AppendText("Bot: " + botResponse + "\n");
            txtInput.Clear();*/
        }
        private string GetBotResponse(string input)
        {
            // Aqui você pode implementar a lógica do seu chatbot
            // Por enquanto, vamos retornar uma resposta simples
            switch (input.ToLower())
            {
                case "oi":
                    return "Olá! Como posso te ajudar hoje?";
                case "como voce esta?":
                    return "Sou apenas um programa, mas estou funcionando bem!";
                default:
                    return "Desculpe, eu não entendi. Pode repetir?";
            }
        }
    }
}

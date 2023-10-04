using System.Text;
using SkiaSharp;

namespace Projeto
{
	public class Forma
	{
		private class ElementoPilha
		{
			public readonly int X, Y, I;

			public ElementoPilha(int x, int y, int i)
			{
				X = x;
				Y = y;
				I = i;
			}
		}

		private static unsafe void ConsumirForma(byte* imagem, int largura, int altura, Forma forma, Stack<ElementoPilha> pilha, bool considerar8Vizinhos, Forma[]? mapa = null)
		{
			while (pilha.Count != 0)
			{
				int x, y, i, oldI;
				ElementoPilha e = pilha.Pop();

				x = e.X;
				y = e.Y;

				// Verifica acima
				i = e.I - largura;
				if (y > 0 && imagem[i] == 255)
				{
					forma.AdicionarPixel(x, y - 1);
					if (mapa != null)
					{
						mapa[i] = forma;
					}
					imagem[i] = 254;
					pilha.Push(new ElementoPilha(x, y - 1, i));
				}

				// Verifica abaixo
				i = e.I + largura;
				if (y < (altura - 1) && imagem[i] == 255)
				{
					forma.AdicionarPixel(x, y + 1);
					if (mapa != null)
					{
						mapa[i] = forma;
					}
					imagem[i] = 254;
					pilha.Push(new ElementoPilha(x, y + 1, i));
				}

				// Vai tudo até a esquerda, verificando acima e abaixo
				x--;
				i = e.I - 1;
				while (x > 0 && imagem[i] == 255)
				{
					forma.AdicionarPixel(x, y);
					if (mapa != null)
					{
						mapa[i] = forma;
					}
					imagem[i] = 254;

					oldI = i;

					// Verifica acima
					i = oldI - largura;
					if (y > 0 && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y - 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y - 1, i));
					}

					// Verifica abaixo
					i = oldI + largura;
					if (y < (altura - 1) && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y + 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y + 1, i));
					}

					i = oldI - 1;
					x--;
				}

				// Última verificação (porque utilizamos os 8 vizinhos): as diagonais
				if (considerar8Vizinhos && x >= 0)
				{
					oldI = i;

					// Verifica acima
					i = oldI - largura;
					if (y > 0 && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y - 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y - 1, i));
					}

					// Verifica abaixo
					i = oldI + largura;
					if (y < (altura - 1) && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y + 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y + 1, i));
					}
				}

				// Agora, vai tudo até a direita, verificando acima e abaixo
				x = e.X + 1;
				i = e.I + 1;
				while (x < largura && imagem[i] == 255)
				{
					forma.AdicionarPixel(x, y);
					if (mapa != null)
					{
						mapa[i] = forma;
					}
					imagem[i] = 254;

					oldI = i;

					// Verifica acima
					i = oldI - largura;
					if (y > 0 && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y - 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y - 1, i));
					}

					// Verifica abaixo
					i = oldI + largura;
					if (y < (altura - 1) && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y + 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y + 1, i));
					}

					i = oldI + 1;
					x++;
				}

				// Última verificação (porque utilizamos os 8 vizinhos): as diagonais
				if (considerar8Vizinhos && x < largura)
				{
					oldI = i;

					// Verifica acima
					i = oldI - largura;
					if (y > 0 && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y - 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y - 1, i));
					}

					// Verifica abaixo
					i = oldI + largura;
					if (y < (altura - 1) && imagem[i] == 255)
					{
						forma.AdicionarPixel(x, y + 1);
						if (mapa != null)
						{
							mapa[i] = forma;
						}
						imagem[i] = 254;
						pilha.Push(new ElementoPilha(x, y + 1, i));
					}
				}
			}
		}

		public static unsafe List<Forma> CriarMapaDeFormas(byte* imagem, int largura, int altura, bool considerar8Vizinhos, out Forma[] mapaDeFormas)
		{
			List<Forma> formasIndividuais = new List<Forma>();
			Forma[] mapa = new Forma[largura * altura];
			Stack<ElementoPilha> pilha = new Stack<ElementoPilha>(2048);

			int i = 0;
			for (int y = 0; y < altura; y++)
			{
				for (int x = 0; x < largura; x++, i++)
				{
					if (imagem[i] == 255)
					{
						Forma forma = new Forma(x, y);
						pilha.Push(new ElementoPilha(x, y, i));
						imagem[i] = 254;
						mapa[i] = forma;
						ConsumirForma(imagem, largura, altura, forma, pilha, considerar8Vizinhos, mapa);
						forma.AtualizarCentro();
						formasIndividuais.Add(forma);
					}
				}
			}

			mapaDeFormas = mapa;

			return formasIndividuais;
		}

		public static unsafe List<Forma> DetectarFormas(byte* imagem, int largura, int altura, bool considerar8Vizinhos)
		{
			List<Forma> formasIndividuais = new List<Forma>();
			Stack<ElementoPilha> pilha = new Stack<ElementoPilha>(2048);

			int i = 0;
			for (int y = 0; y < altura; y++)
			{
				for (int x = 0; x < largura; x++, i++)
				{
					if (imagem[i] == 255)
					{
						Forma forma = new Forma(x, y);
						pilha.Push(new ElementoPilha(x, y, i));
						imagem[i] = 254;
						ConsumirForma(imagem, largura, altura, forma, pilha, considerar8Vizinhos);
						forma.AtualizarCentro();
						formasIndividuais.Add(forma);
					}
				}
			}

			return formasIndividuais;
		}

		public int Area, X0, Y0, X1, Y1, CentroX, CentroY;

		public int Largura
		{
			get
			{
				return X1 - X0 + 1;
			}
		}

		public int Altura
		{
			get
			{
				return Y1 - Y0 + 1;
			}
		}

		public Forma(int x, int y)
		{
			Area = 1;
			X0 = x;
			Y0 = y;
			X1 = x;
			Y1 = y;
			CentroX = x;
			CentroY = y;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Area: ");
			stringBuilder.Append(Area);
			stringBuilder.Append(" / X0: ");
			stringBuilder.Append(X0);
			stringBuilder.Append(" / Y0: ");
			stringBuilder.Append(Y0);
			stringBuilder.Append(" / X1: ");
			stringBuilder.Append(X1);
			stringBuilder.Append(" / Y1: ");
			stringBuilder.Append(Y1);
			stringBuilder.Append(" / CentroX: ");
			stringBuilder.Append(CentroX);
			stringBuilder.Append(" / CentroY: ");
			stringBuilder.Append(CentroY);
			return stringBuilder.ToString();
		}

		public void AdicionarPixel(int x, int y)
		{
			Area++;
			if (X0 > x)
			{
				X0 = x;
			}
			if (Y0 > y)
			{
				Y0 = y;
			}
			if (X1 < x)
			{
				X1 = x;
			}
			if (Y1 < y)
			{
				Y1 = y;
			}
		}

		public void AtualizarCentro()
		{
			CentroX = (X1 + X0) / 2;
			CentroY = (Y1 + Y0) / 2;
		}

		public bool ContemPonto(int x, int y)
		{
			return (x >= X0 && x <= X1 && y >= Y0 && y <= Y1);
		}

		public bool FazInterseccao(int x0, int y0, int x1, int y1)
		{
			return (x0 <= X1 && x1 >= X0 && y0 <= Y1 && y1 >= Y0);
		}
	}

	class Program
	{

		static unsafe byte* ConverterCinza(byte* entrada, byte* cinza, int total_pixels, out int sum)
		{
			sum = 0;
			for (int e = 0, s = 0; s < total_pixels; e += 4, s++)
			{
				cinza[s] = (byte)((entrada[e] + entrada[e + 1] + entrada[e + 2]) / 3);
				sum += cinza[s];
			}
			return cinza;
		}

		static unsafe byte* Limiarizacao(byte* cinza, byte* limiarizado, int total_pixels, int media)
		{
			for (int e = 0; e < total_pixels; e++)
			{
				if (cinza[e] >= media)
				{
					limiarizado[e] = 255;
				}
				else
				{
					limiarizado[e] = 0;
				}
			}
			return limiarizado;
		}

		static unsafe byte* Invercao(byte* limiarizado, byte* invertido, int total_pixels)
		{
			for (int e = 0; e < total_pixels; e++)
			{
				if (limiarizado[e] == 255)
				{
					invertido[e] = 0;
				}
				else
				{
					invertido[e] = 255;
				}
			}
			return invertido;
		}

		static unsafe byte MenorValorRegiao(byte* entrada, int largura, int altura, int x, int y, int metadeJanela)
		{
			int xInicial = x - metadeJanela;
			int yInicial = y - metadeJanela;
			int xFinal = x + metadeJanela;
			int yFinal = y + metadeJanela;

			if (xInicial < 0)
			{
				xInicial = 0;
			}
			if (yInicial < 0)
			{
				yInicial = 0;
			}
			if (xFinal > largura - 1)
			{
				xFinal = largura - 1;
			}
			if (yFinal > altura - 1)
			{
				yFinal = altura - 1;
			}

			byte menor = entrada[(yInicial * largura) + xInicial];

			for (y = yInicial; y <= yFinal; y++)
			{
				for (x = xInicial; x <= xFinal; x++)
				{

					int i = (y * largura) + x;
					if (entrada[i] < menor)
					{
						menor = entrada[i];
					}

				}
			}

			return menor;
		}

		static unsafe byte* SemErosao(byte* invertido, byte* sem_erosao, int largura, int altura)
		{
			int tamanhoJanela = 9;
			int metadeJanela = tamanhoJanela / 2;

			for (int y = 0; y < altura; y++)
			{
				for (int x = 0; x < largura; x++)
				{
					int i = (y * largura) + x;
					sem_erosao[i] = MenorValorRegiao(invertido, largura, altura, x, y, metadeJanela);
				}
			}

			return sem_erosao;
		}

		static void Main(string[] args)
		{
			using (SKBitmap bitmapEntrada = SKBitmap.Decode("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_1.png"),
				bitmapCinza = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapLimiarizado = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapInvertido = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapSemErosao = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)))
			{

				unsafe
				{
					byte* entrada = (byte*)bitmapEntrada.GetPixels();
					byte* cinza = (byte*)bitmapCinza.GetPixels();
					byte* limiarizado = (byte*)bitmapLimiarizado.GetPixels();
					byte* invertido = (byte*)bitmapInvertido.GetPixels();
					byte* sem_erosao = (byte*)bitmapSemErosao.GetPixels();

					int altura = bitmapEntrada.Height;
					int largura = bitmapEntrada.Width;

					int total_pixels_entrada = altura * largura;
					int sum = 0;

					cinza = ConverterCinza(entrada, cinza, total_pixels_entrada, out sum);

					int media = sum / total_pixels_entrada;

					limiarizado = Limiarizacao(cinza, limiarizado, total_pixels_entrada, media);
					invertido = Invercao(limiarizado, invertido, total_pixels_entrada);
					sem_erosao = SemErosao(invertido, sem_erosao, largura, altura);

				}

				using (FileStream stream = new FileStream("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_1_cinza.png", FileMode.OpenOrCreate, FileAccess.Write))
				{
					bitmapCinza.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
				using (FileStream stream = new FileStream("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_1_limiarizado.png", FileMode.OpenOrCreate, FileAccess.Write))
				{
					bitmapLimiarizado.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
				using (FileStream stream = new FileStream("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_1_invertido.png", FileMode.OpenOrCreate, FileAccess.Write))
				{
					bitmapInvertido.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
				using (FileStream stream = new FileStream("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_1_sem_erosao.png", FileMode.OpenOrCreate, FileAccess.Write))
				{
					bitmapSemErosao.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
			}
		}
	}
}


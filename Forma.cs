using System.Globalization;
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

	public class AreaValidacao
	{
		public int X0 { get; set; }
		public int Y0 { get; set; }
		public int X1 { get; set; }
		public int Y1 { get; set; }

		public AreaValidacao(int x0, int y0, int x1, int y1)
		{
			X0 = x0;
			Y0 = y0;
			X1 = x1;
			Y1 = y1;
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
			int tamanhoJanela = 7;
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

		static unsafe byte* RegularizaImagem(byte* entrada, int altura, int largura)
		{
			using (SKBitmap bitmapRegularizado = new SKBitmap(new SKImageInfo(largura, altura, SKColorType.Gray8)))
			{
				byte* cinza = (byte*)bitmapRegularizado.GetPixels();

				int total_pixels_entrada = altura * largura;
				int sum = 0;

				cinza = ConverterCinza(entrada, cinza, total_pixels_entrada, out sum);

				int media = sum / total_pixels_entrada;

				byte* limiarizado = (byte*)bitmapRegularizado.GetPixels();
				limiarizado = Limiarizacao(cinza, limiarizado, total_pixels_entrada, media);

				byte* invertido = (byte*)bitmapRegularizado.GetPixels();
				invertido = Invercao(limiarizado, invertido, total_pixels_entrada);

				byte* sem_erosao = (byte*)bitmapRegularizado.GetPixels();
				byte* regularizado = (byte*)bitmapRegularizado.GetPixels();
				regularizado = SemErosao(invertido, sem_erosao, largura, altura);

				return regularizado;
			}
		}
		static bool ValidacaoProva(List<Forma> lista_formas, List<AreaValidacao> lista_areas_quadrados_validacao)
		{
			int quadrados_validacao_prova = 0;
			for (int i = 0; i < lista_formas.Count; i++)
			{
				for (int j = 0; j < lista_areas_quadrados_validacao.Count; j++)
				{
					if (lista_formas[i].FazInterseccao(lista_areas_quadrados_validacao[j].X0, lista_areas_quadrados_validacao[j].Y0, lista_areas_quadrados_validacao[j].X1, lista_areas_quadrados_validacao[j].Y1))
					{
						quadrados_validacao_prova++;
					}
				}
			}
			// Console.WriteLine("Quadrados de validação: " + quadrados_validacao_prova);
			if (quadrados_validacao_prova != 6)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		static void Main(string[] args)
		{
			using (SKBitmap bitmapEntrada = SKBitmap.Decode("/Users/rafael/Documents/University/2023-02/computacao_cognitiva/atividade_grupo_1/gabarito_correto_3.png"),
				bitmapCinza = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapLimiarizado = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapInvertido = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)),
				bitmapSemErosao = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8)))
			{
				// Console.WriteLine(bitmapEntrada.ColorType);
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

					// byte* regularizado = RegularizaImagem(entrada, altura, largura);

					List<Forma> lista_formas = Forma.DetectarFormas(sem_erosao, largura, altura, true);

					List<AreaValidacao> lista_areas_quadrados_validacao = new List<AreaValidacao>();
					lista_areas_quadrados_validacao.Add(new AreaValidacao(30, 30, 84, 84));
					lista_areas_quadrados_validacao.Add(new AreaValidacao(637, 30, 691, 84));
					lista_areas_quadrados_validacao.Add(new AreaValidacao(30, 494, 84, 547));
					lista_areas_quadrados_validacao.Add(new AreaValidacao(637, 494, 691, 547));
					lista_areas_quadrados_validacao.Add(new AreaValidacao(30, 957, 84, 1011));
					lista_areas_quadrados_validacao.Add(new AreaValidacao(637, 957, 691, 1011));

					// Console.WriteLine("Formas encontradas: " + lista_formas.Count);

					bool validacao_prova = ValidacaoProva(lista_formas, lista_areas_quadrados_validacao);

					if (validacao_prova)
					{
						Console.WriteLine("Gabarito correto!");

						Dictionary<int, List<String>> respostas = new Dictionary<int, List<String>>();
						respostas.Add(0, new List<String>());
						respostas.Add(1, new List<String>());
						respostas.Add(2, new List<String>());
						respostas.Add(3, new List<String>());
						respostas.Add(4, new List<String>());
						respostas.Add(5, new List<String>());
						respostas.Add(6, new List<String>());
						respostas.Add(7, new List<String>());
						respostas.Add(8, new List<String>());
						respostas.Add(9, new List<String>());


						List<int> lista_posicao_x0 = new List<int>();
						List<int> lista_posicao_y0 = new List<int>();
						List<int> lista_posicao_x1 = new List<int>();
						List<int> lista_posicao_y1 = new List<int>();


						lista_posicao_x0.Add(207);
						lista_posicao_x0.Add(272);
						lista_posicao_x0.Add(337);
						lista_posicao_x0.Add(402);
						lista_posicao_x0.Add(468);


						lista_posicao_y0.Add(330);
						lista_posicao_y0.Add(394);
						lista_posicao_y0.Add(459);
						lista_posicao_y0.Add(524);
						lista_posicao_y0.Add(588);
						lista_posicao_y0.Add(653);
						lista_posicao_y0.Add(718);
						lista_posicao_y0.Add(782);
						lista_posicao_y0.Add(847);
						lista_posicao_y0.Add(912);


						lista_posicao_x1.Add(251);
						lista_posicao_x1.Add(316);
						lista_posicao_x1.Add(381);
						lista_posicao_x1.Add(447);
						lista_posicao_x1.Add(512);


						lista_posicao_y1.Add(374);
						lista_posicao_y1.Add(439);
						lista_posicao_y1.Add(503);
						lista_posicao_y1.Add(568);
						lista_posicao_y1.Add(633);
						lista_posicao_y1.Add(697);
						lista_posicao_y1.Add(762);
						lista_posicao_y1.Add(827);
						lista_posicao_y1.Add(891);
						lista_posicao_y1.Add(956);

						for (int forma_aux = 0; forma_aux < lista_formas.Count; forma_aux++)
						{
							for (int y_aux = 0; y_aux < 10; y_aux++)
							{
								for (int x_aux = 0; x_aux < 5; x_aux++)
								{
									if (x_aux == 0 && lista_formas[forma_aux].FazInterseccao(lista_posicao_x0[x_aux], lista_posicao_y0[y_aux], lista_posicao_x1[x_aux], lista_posicao_y1[y_aux]))
									{
										respostas[y_aux].Add("A");
									}
									else if (x_aux == 1 && lista_formas[forma_aux].FazInterseccao(lista_posicao_x0[x_aux], lista_posicao_y0[y_aux], lista_posicao_x1[x_aux], lista_posicao_y1[y_aux]))
									{
										respostas[y_aux].Add("B");
									}
									else if (x_aux == 2 && lista_formas[forma_aux].FazInterseccao(lista_posicao_x0[x_aux], lista_posicao_y0[y_aux], lista_posicao_x1[x_aux], lista_posicao_y1[y_aux]))
									{
										respostas[y_aux].Add("C");
									}
									else if (x_aux == 3 && lista_formas[forma_aux].FazInterseccao(lista_posicao_x0[x_aux], lista_posicao_y0[y_aux], lista_posicao_x1[x_aux], lista_posicao_y1[y_aux]))
									{
										respostas[y_aux].Add("D");
									}
									else if (x_aux == 4 && lista_formas[forma_aux].FazInterseccao(lista_posicao_x0[x_aux], lista_posicao_y0[y_aux], lista_posicao_x1[x_aux], lista_posicao_y1[y_aux]))
									{
										respostas[y_aux].Add("E");
									}
								}
							}
						}

						for (int i = 0; i < 10; i++)
						{
							String resposta = "Questão " + (i + 1) + ": ";
							if (respostas[i].Count == 0)
							{
								resposta += "Nenhuma alternativa";
							}
							else
							{
								for (int j = 0; j < respostas[i].Count; j++)
								{
									resposta += respostas[i][j];
									if (j < (respostas[i].Count - 1))
									{
										resposta += ", ";
									}
								}

							}
							Console.WriteLine(resposta);
						}
					}
					else
					{
						Console.WriteLine("Gabarito errado!");
					}
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


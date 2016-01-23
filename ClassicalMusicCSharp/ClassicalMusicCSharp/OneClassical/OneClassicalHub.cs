using ClassicalMusicCSharp.Classes.Radio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace ClassicalMusicCSharp.OneClassical
{
    public class OneClassicalHub
    {
        private static OneClassicalHub instance;

        public static OneClassicalHub Instance
        {
            get
            {
                if (instance == null)
                    instance = new OneClassicalHub();
                return instance;
            }
        }
        public bool Loaded { get; set; } = false;
        private OneClassicalHub() { }
        public List<Compositore> ListaCompositori { get; private set; }
        public async Task<List<Compositore>> readJson()
        {
            if (!Loaded)
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///music.json"));
                string fileContent;
                using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    fileContent = await sRead.ReadToEndAsync();
                    sRead.Dispose();
                }

                JsonArray listComp = JsonArray.Parse(fileContent);
                List<Compositore> compositori = new List<Compositore>();
                for (int i = 0; i < listComp.Count; i++)
                {
                    JsonObject jComp = listComp[i].GetObject();
                    string nome = jComp["nome"].GetString();
                    Compositore comp = new Compositore() { Nome = nome };

                    if (jComp.ContainsKey("categorie"))
                    {
                        JsonArray jCategorie = jComp["categorie"].GetArray();
                        List<Categoria> listCategorie = new List<Categoria>(jCategorie.Count);
                        for (int j = 0; j < jCategorie.Count; j++)
                        {
                            JsonObject jcat = jCategorie[j].GetObject();
                            string nomeCat = jcat["categoria"].GetString();
                            Categoria categoria = new Categoria() { Nome = nomeCat };

                            JsonArray jOpere = jcat["opere"].GetArray();
                            List<Opera> listOpere = new List<Opera>(jOpere.Count);
                            for (int z = 0; z < jOpere.Count; z++)
                            {
                                JsonObject jOpera = jOpere[z].GetObject();
                                string nomeOpera = jOpera["opera"].GetString();

                                JsonArray jListTracce = jOpera["tracce"].GetArray();
                                List<Traccia> tracce = new List<Traccia>(jListTracce.Count);

                                Opera opera = new Opera() { Nome = nomeOpera, Tracce = tracce};

                                for (int x = 0; x < jListTracce.Count; x++)
                                {
                                    JsonObject jtraccia = jListTracce[x].GetObject();
                                    string titolo = jtraccia["titolo"].GetString();
                                    string link = jtraccia["link"].GetString();
                                    Traccia track = new Traccia() { Titolo = titolo, Link = link};
                                    tracce.Add(track);
                                }
                                listOpere.Add(opera);
                            }
                            categoria.Opere = listOpere;
                            listCategorie.Add(categoria);
                        }
                        comp.Categorie = listCategorie;
                    }

                    if (jComp.ContainsKey("opere"))
                    {
                        JsonArray jOpere = jComp["opere"].GetArray();
                        List<Opera> listOpere = new List<Opera>(jOpere.Count);
                        for (int j = 0; j < jOpere.Count; j++)
                        {
                            JsonObject jOpera = jOpere[j].GetObject();
                            string nomeOpera = jOpera["opera"].GetString();

                            JsonArray jListTracce = jOpera["tracce"].GetArray();
                            List<Traccia> tracce = new List<Traccia>(jListTracce.Count);
                            Opera opera = new Opera() { Nome = nomeOpera, Tracce = tracce};
                            for (int z = 0; z < jListTracce.Count; z++)
                            {
                                JsonObject jtraccia = jListTracce[z].GetObject();
                                string titolo = jtraccia["titolo"].GetString();
                                string link = jtraccia["link"].GetString();
                                Traccia track = new Traccia() { Titolo = titolo, Link = link};
                                tracce.Add(track);
                            }
                            listOpere.Add(opera);
                        }
                        comp.Opere = listOpere;
                    }
                    compositori.Add(comp);
                }
                ListaCompositori = compositori;
                Loaded = true;
            }
            await RadioManager.LoadRadio();
            return ListaCompositori;
        }
    }
}

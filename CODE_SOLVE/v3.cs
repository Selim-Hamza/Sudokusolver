using System;
using System.Collections.Generic;
using System.IO;


class V3
{

  public static void Main(string[] args)
  {
    /*
    int[,] sudoTest = new int[,]
    {
      { 8, 0, 0, 0, 0, 0, 0,0 , 0 },
      { 0, 0, 3, 6, 0, 0, 0, 0, 0 },
      { 0, 7, 0, 0, 9, 0, 2, 0, 0 },
      { 0, 5, 0,0 , 0, 7,0 ,0 , 0 },
      { 0, 0, 0, 0, 4, 5, 7, 0, 0 },
      { 0, 0, 0, 1, 0, 0, 0, 3,0  },
      { 0, 0, 1, 0, 0, 0, 0, 6, 8 },
      { 0, 0, 8, 5, 0, 0, 0, 1, 0 },
      { 0, 9, 0, 0, 0, 0, 4, 0,  0}
    };

    int[,] sudoTest2 = new int[,]
    {
      { 0, 0, 0, 6, 1, 4, 0, 0, 0 },
      { 9, 0, 8, 0, 0, 0, 0, 0, 0 },
      { 0, 1, 0, 8, 0, 0, 7, 3, 0 },
      { 2, 0, 0, 9, 6, 0, 0, 0, 0 },
      { 0, 7, 0, 3, 0, 0, 9, 0, 5 },
      { 0, 6, 0, 0, 0, 0, 0, 1, 2 },
      { 0, 0, 3, 0, 0, 0, 8, 2, 0 },
      { 4, 0, 0, 0, 9, 2, 0, 0, 0 },
      { 0, 0, 5, 7, 0, 8, 6,0, 9 }
    };
    */

    Console.WriteLine("Saisir format : ");
    int format = int.Parse(Console.ReadLine());

    int[,] sudoTest = new int[format,format];

    affiche(sudoTest);

    Console.WriteLine("--------------------------");

    //legal(sudoTest);

    Console.WriteLine("--------------------------");

    //affiche(sudoTest);

    Console.WriteLine("--------------------------");

    genereGrille("simple.txt", sudoTest); //Modifier fichier en fonction du format --> ici 9*9

    legal(sudoTest);

    affiche(sudoTest);



  }

  /*
    genereGrille : proc
    genere des grilles de sudoku à partir d'un fichier texte
    le nom du fishier doit etre passé en parametre
    Parametres : fic : string : le nom du fichier
                sudoVide : int[,] : tableau 2D --> grille à remplir
  */
  static void genereGrille(string fic, int[,] sudoVide)
  {

    StreamReader sr = new StreamReader(fic); //permet de lire le fichier

    string line = sr.ReadLine();

    string[] tabval = line.Split(' ');
    List<int> lstVal =  new List<int>();

    //Remplissage d'une liste avec toutes les valeur caster en int
    for (int i = 0; i<tabval.Length; i++)
    {
      int val = int.Parse(tabval[i]);
      lstVal.Add(val);
    }

    //Ajout des valeurs dans la grille
    for(int i = 0; i< sudoVide.GetLength(0); i++)
    {
      for(int j = 0; j< sudoVide.GetLength(1); j++)
      {
          sudoVide[i,j] = lstVal[0];
          lstVal.RemoveAt(0);
      }

    }

    sr.Close();

  }



  /*
    Unique : fonct : bool
    Complete partiellement ou completement un sudoku en fonction de sa difficulté
    avec technique de l'unicité
    Retourne vrai si completement completer / Faux si partiellement completé
    Parametres : int[,] sudo : tableau 2D representant la grille de sudoku à resoudre
  */

  static bool unique(int[,] sudo)
  {

    bool res = false; //Resultat de la resolution

    List<int> lstCandit = new List<int>();  // Liste des chiffres cadidat possibles

    int nbLignes = sudo.GetLength(0);   // Nb lignes du tableau
    int nbCol = sudo.GetLength(1);      // Nb col du tableau

    int i = 0; //Compteur ligne
    int j = 0;  //Compteur col

    bool remplie = false; //Verifie si sudoku resolu
    bool trpDur = false; //Certaines grilles ne pourrons pas etre complété par cette fonction --> true si la resolution bloque

    /*
     Parcourt de chaque case vide avec while car nous allons
     incrémenter/décrémenter nos compteur avec des ref de manière différente
    */
    while (i < nbLignes && remplie == false && trpDur == false)      // Parcourt de chaque lignes
    {
        while (j < nbCol && remplie == false && trpDur == false)     // Parcourt de chaque col
        {
            int actuel = sudo[i, j]; //Pr pas repeter trop de fois

            lstCandit.Clear();      // On vide la liste des cndidats de la case precedente

            if (actuel == 0)
            {
                for (int nombre = 1; nombre <= nbCol; nombre++)     // Si la case est vide --> vin verifie chaque nombres possible sur la case
                {


                    if (isValid(sudo,  i, j, nombre))        //isValid verifie si nombres pas pressent sur ligne/col/region
                    {
                      lstCandit.Add(nombre);
                    }      // Ajout du nombre dans la liste de lstCandit
                  }
              }

              // S'il n'y a qu'un seul candidat pour cette case, alors ce candidat est forcément le bon
              if (lstCandit.Count == 1)
              {
                  sudo[i, j] = lstCandit[0];   // S'il n'y a qu'une valeur, alors celle-ci se trouve forcément à la 1ère position de la liste


                  // Nous vérifions si la grille est complète
                  if (complete(sudo))
                  {
                      remplie = true;      // Nous avons résolu le sudoku
                  }
                  // Retour au debut du sudoku car sa modification à potentiellement créer des nouvelles solutions uniques
                  // D'ou boucle while et pas for pour le parcour
                  i = 0;
                  j = 0;
              }

                // CAS OU IL Y A PLUSIEUR CANDIDATS
                else
                {
                    if (dernier_Vide(sudo, i, j) == false) //SI pas la derniere case su sudoku --> on determine et examine la suivante
                    {
                        caseSuivante(sudo, ref i, ref j); //ref pour pouvoir modifier i et j et donner coord de la prochaine case à examiner
                    }
                    else
                    {
                        if (complete(sudo) == false) // Si nous sommes à la dernière case vide et sudo toujour incomplet --> resolution total impossible
                        {
                            trpDur = true;
                        }
                    }
                }

            }
        }

    if(trpDur)
    {
      // Si grille pas completable --> res = false
      res = false;
    }
    else       // Sinon grille resolu avec unicité (rare)
    {
        res = true;
    }

    return res;
  }




  static bool complete(int[,] sudo)
  {
    bool res = true;
    bool vide = false; // Pour eviter de parcourir toute la grille si on vide une case vide

    // Parcourt de la grille
    for (int i = 0; i < sudo.GetLength(0) && vide == false; i++)
    {
      for (int j = 0; j < sudo.GetLength(1) && vide == false; j++)
      {
        if (sudo[i, j] == 0) // Si la case est vide --> grille incomplete --> res = false
        {
          res = false;
          vide = true; //Pour sortir du parcour de la boucle sans double return illegal ;-)
        }
      }
    }

    return res;
  }


    /*
      dernier_Vide : fonct: bool
      Fonction renvoi vrai si les coordonnées de la case passé en parametre sont celle
      de la derniere case vide // Faux sinon --> Utile pour d'autre fonction

      Parametres : int[,] sudo : la grille de sudoku
                    li : int : coordonnés de la ligne du chiffre a comparer avec la derniere case vide
                    col : int : coordonnés de la col du chiffre a comparer avec la derniere case vide
    */
    static bool dernier_Vide(int[,] sudo, int li, int col)
    {
        bool res = false;


        // Declaration et Initialisation des coordonnées de la derniere case vide
        int liVide= 0;
        int colVide = 0;

        //Parcour de la grille
        for (int i = 0; i < sudo.GetLength(0); i++)
        {
            for (int j = 0; j < sudo.GetLength(1); j++)
            {
                if (sudo[i, j] == 0)  // Recuperation des coordonnés de la derniere case vide
                {
                    liVide= i;
                    colVide = j;
                }
            }
        }

        // Comparaison avec les coordonnées fournis en paramètre :

        if (li == liVide&& col == colVide)
        {
            res = true;
        }

        return res;
    }


    /*
      isValid : fonct : bool
      Renvoie vrai si le candidat est placable aux coordonnés passé en parametre en verifiant ligne / colonne / region
      Renvoie faux sinon
      Parametres : sudo : int[,] : tableau 2D representant la grille de sudoku à resoudre
                  li : int : coordonnés de la ligne à verif
                  col : int : coordonnés de la colonne à verif
                  cand : int : Entier representant le candidat
    */
    static bool isValid(int[,] sudo, int li, int col, int cand)
    {
      bool res = true; // resultat
      int taille = sudo.GetLength(0); //Format de la grille
      double racD = Math.Sqrt(taille); //Racine carré du format --> utile pour verifier les zones
      int rac = (int)racD; // cast de la racine carré en int


      //Parcour de la grille et verif si candidats est pressent dans la ligne / colonne / region
      for (int i = 0; i < taille; i++)
      {
        //VERIF LIGNE
        if (sudo[i, col] != 0 && sudo[i, col] == cand)
        {
          res = false;
        }

        //VERIF CLONNNE
        if (sudo[li, i] != 0 && sudo[li, i] == cand)
        {
          res = false;
        }

        //VERIF ZONE avec RACINE CARRE du FORMAT
        if (sudo[rac *(li / rac) + i / rac , rac * (col / rac) + i % rac] != 0 && sudo[rac *(li / rac) + i/rac, rac*(col / rac) + i % rac] == cand)
        {
          res = false;
        }

      }

        return res;
    }


    /*
      caseSuivante : proc
      determine la prochaine case d'une grille de sudoku a partir des coordonnées ref en parametre
      Parametres : sudo : int[,] : tableau 2D representant grille de sudoku
                  li : ref de int : ref de coordonnés de la ligne d'une case
                  col : ref de int : ref de coordonnés de la colonne d'une case
    */
    static void caseSuivante(int[,] sudo, ref int li, ref int col)
    {
        if (col == sudo.GetLength(0) - 1) // Si nous sommes à la dernière col de la ligne
        {
          col = 0;
          li++;  // Nous allons à la  1ere colonne de la ligne suivante --
        }
        else
        {
          col++; //Sinon passe col suivante
        }
    }

    /*
      affiche : proc
      Affiche tableau 2D passé en parametre (grille de sudoku)
      Parametres : sudo : int[,] : grille de sudoku
    */
    static void affiche(int[,] sudo)
    {
      for (int i = 0; i < sudo.GetLength(0); i++)
      {
        for (int j = 0; j < sudo.GetLength(1); j++)
        {
          Console.Write(sudo[i,j]+" ");
        }
        Console.WriteLine();
      }
    }



    /*
      legal : fonct : bool
      Fonction permetant de resoudre des sudoku en testant toutes les
      posibilté et en eregistrant les coordonnés des cases tésté pour éviter
      la frauduleuse récurssivité ;-)
      Parametres : sudo : int[,] : grille de sudoku à resoudre
    */
    static bool legal(int[,] sudo)
    {
      bool res = false;   // True si resolu / False sinon


      int valDeBase = 0;     //  variable permettent de stocker l'ancienne valeur d'une case afin de revenir dessus pour tester les autres valeur une fois bloqué
      List<string> lstRemplies = new List<string>();   // Liste contenant toutes les cases (coordonnées + valeur de la case) où nous avons inséré une valeur

      bool resolu = false;    //booléen permetant d'arrêter si sudoku résolu --> Optimisation --> gain de temps dans résolutions
      bool impossible = false; // au cas ou nous testions des fausses grilles sur internet

      int i = 0;      // Ligne où l'on se situe dans la grille de sudoku
      int j = 0;      // Colonne où l'on se situe dans la grille de sudoku

      int posLigne = sudo.GetLength(0);
      int posCol = sudo.GetLength(1);

        while (i < posLigne && resolu == false && impossible == false )  // Parcourt de chaque ligne
        {
            while (j < posCol && resolu == false && impossible == false)  // Parcourt de chaque colonne
            {

              if (sudo[i, j] == 0) // Si la case est vide
              {
                    bool auMoinUnCandid = false; //booléen permettant de verifier si nous avons deja inséré au moins un candidat à cette position
                    bool candidTrouve = false;      // booléen permetant d'arrêter recherche quand candidat trouvé

                    for (int cherchCandid = 1; cherchCandid <= posLigne && candidTrouve == false; cherchCandid++)        // Parcourt de tous les nombres possibles sur cette grille
                    {

                        // Des que nous sommes bloqués , on cherche les autres possiblités à partir de la prochaine valeur car valDeBase commence à 0
                        if (cherchCandid > valDeBase)
                        {
                            // Si nb pas present sur colonne/ligne et region
                            if (isValid(sudo, i , j, cherchCandid))
                            {
                                candidTrouve = true;
                                auMoinUnCandid = true;

                                sudo[i, j] = cherchCandid;   // Insertion du candidat dans la case

                                //sauvegarde des données de la case (ligne,colonne,valeur de la case) pour pouvoir revenir dessus si bloqué
                                string dataCase = i + "," + j + "," + cherchCandid;
                                lstRemplies.Add(dataCase); // ->   ajout des données a la liste

                                valDeBase = 0;  // Réinitialisation de la valeur de Base pour continuer la suite des test
                              }
                          }
                        }

                    if (auMoinUnCandid)  // S'il y a au moins un candidat trouvé et inserer,
                    {
                        caseSuivante(sudo, ref i, ref j); //On passe à la prochaine case
                    }
                    else       // Sinon 2 cas possible
                    {
                        if (lstRemplies.Count > 0)       // 1er CAS : S'il y a au moins une case tésté
                        {
                            //on récuper les coordonnées de la dernière case tésté
                            int dernierTest = lstRemplies.Count-1;

                            string[] t = lstRemplies[dernierTest].Split(',');

                            lstRemplies.RemoveAt(dernierTest);  // Supression de la case car on rééxamine
                            // Une fois les coordonnées récupérées, nous revenons à cette case
                            i = int.Parse(t[0]); //Car coordonées ds une string à la base
                            j = int.Parse(t[1]);

                            valDeBase = int.Parse(t[2]);       //Pour retest à la bonne valeur

                            sudo[i, j] = 0;      // remise à 0
                        }
                        // 2eme CAS : On peut revenir sur aucune case --> GRILLE IMPOSSIBLE
                        else       // Sinon s'il n'y a aucune case sur laquelle nous pouvons revenir dessus --> alors cette grille n'est pas solvable
                       {
                        impossible = true;
                       }
                    }
                }
                else // Si case déja reseigné on passe à la case suivante
                {
                    // Nous passons à la case suivante
                    caseSuivante(sudo, ref i, ref j);
                }

                //  verification si resolu pour pas boucler dans le vide
                if (complete(sudo))
                {
                    resolu = true;
                    res = true;
                }


            }
          }
            return res;
          }


          /*
            illegal : fonct : bool
            Fonction illegalement reccursive mais optimisé résolvant des grilles de SUDOKU
            Parametres : sudo : int[,] : tableau 2D representant la grille de sudoku à resoudre
          */
          public static bool illegal(int[,] sudo)
          {
            int format = sudo.GetLength(0); //FORMAT DU SUDOKU
             //CARACTERE CORRESPONDANT AU CHIFFRE MAX QUE PEUX CONTENIR UNE CASE

             int canditMax = 0;
            if (format == 4)
            {
               canditMax = 4;
            }
            else if(format == 9)
            {
               canditMax = 9;
            }
            else
            {
              canditMax = 16;
            }

            //PARCOUR DU SUDOKU
            for (int i = 0; i< sudo.GetLength(0); i++)
            {
              for (int j = 0; j < sudo.GetLength(1); j++)
              {
                //SI CASE VIDE
                if (sudo[i, j] == 0)
                {
                  //BOUCLE AVEC CANDIDAT POSSIBLE
                  for (int candit = 1; candit <= canditMax ; candit++)
                  {
                    //SI CANDIDAT EST VALID --> Ajouter
                    if (isValid(sudo , i , j, candit))
                    {
                      sudo[i,j] = candit;

                      //RECURSIVITE
                      if(illegal(sudo) == true)
                      {
                        return true;
                      }
                      else //Sinon remise de la case à 0 et retest
                      {
                        sudo[i,j] = 0;
                      }
                    }
                  }
                  return false;
                }
              }
            }

            return true;
          }

}

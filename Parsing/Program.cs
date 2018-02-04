/*
 * TELKOM UNIVERSITY
 * User: KELOMPOK 2 TBA (IF 40-06)
 * Date: 11/15/2017
 * Time: 10:22 AM  
 * Ahmad Arsyel Abdul Hakim
 * 
 */
using System;
using System.Collections.Generic;

namespace Parsing
{
	public class Program
	{
		enum State { NUM, OPERATOR, INIT, ERROR, RPAR, LPAR, ACCEPTED } //State yang diperlukan		
		static readonly List<State> lexical = new List<State>(); //Menyimpan jenis lexical pada list
		static readonly List<string> stored = new List<string>(); //Menyimpan string dari lexical pada list 

		
		static bool IsNumber(string currChar){ //Fungsi untuk menentukan apakah char yang dicek merupakan number atau bukan
			if (currChar == "1" || currChar == "2" || currChar == "3" || currChar == "4" || currChar == "5" ||
			    currChar == "6" || currChar == "7" || currChar == "8" || currChar == "9" || currChar == "0"){
				return true;
			}
			return false;
		}

		static string Insert(State stat, string str){ //Fungsi untuk menyimpan state dan string pada list, lalu men-set ulang string
			lexical.Add(stat);
			stored.Add(str);
			return "";
		}
		
		public static void Main(string[] args)
		{			
			Console.Write("Input: ");
			string store = ""; //String untuk menampung char yang sedang di-cek
			State state = State.INIT; //Inisiasi state or start state
			string str = Console.ReadLine(); //Inputan string dari user
			int EOS = str.Length, //Menyimpan panjang string yang diinputkan
			i = 0; //Iterator
			string currChar, nextChar, prevChar; //String untuk menyimpan char yang sedang dicek, setelahnya, dan sebelumnya
			bool comma = false, Esym = false; //Penanda apakah di string inputan telah terdapat (E) atau (,)
			#region Lexical Analyzer										
			do { //Perulangan
				prevChar = i-1 < 0? null : str.Substring(i-1,1); //Mengambil char di iterator sebelumnya, null jika iterator = 0
				currChar = str.Substring(i,1); //Mengambil char di iterator sekarang
				nextChar = i+1 >= EOS? null : str.Substring(i+1,1);	//Mengambil di itertor setelahnya, null jika iterator => EOS 
				
				if (IsNumber(currChar)){ //Jika char sekarang adalah number
					store += currChar; //store menambahkan string currChar 
					if (nextChar == null || nextChar == " " || nextChar == "(" || nextChar == ")"){ //Jika di depan nextChar = null or spasi or kurung 
						if (prevChar == "x" || prevChar == ":"){
							state = State.ERROR;							
						}
						else {
							state = State.NUM;							
						}
						store = Insert(state,store);
					}
				}
				else if (currChar == "E"){ //Jika char sekarang adalah E
					store += currChar;
					if ((!IsNumber(prevChar) || prevChar == " " || prevChar == "+" || prevChar == "-") && !Esym){
						state = State.NUM;					
						if (!IsNumber(prevChar) && (nextChar == "+" || nextChar == "-" || IsNumber(nextChar))){
							state = State.NUM;
						}
						else {
							state = State.ERROR;
							store = Insert(state,store);
						}
					}
					else {
						if ((IsNumber(nextChar) == true) || (nextChar == "+" || nextChar == "-")){
							state = State .NUM;							
						}
						else {
							state = State.ERROR;
							store = Insert(state,store);
						}
					}
					Esym = true;
				}
				else if (currChar == ","){ //Jika char sekarang adalah koma
					store += currChar;
					if (Esym == false){
						if (!(IsNumber(prevChar) && IsNumber(nextChar)) || comma == true){
							state = State.ERROR;
							store = Insert(state,store);
						}
					}
					else {
						state = State.ERROR;
						store = Insert(state,store);
					}
					comma = true;
				}
				else if (currChar == "+" || currChar == "-"){ //Jika char sekarang adalah + atau -
					store += currChar;
					if (prevChar == null){
						if (IsNumber(nextChar) == true){
							state = State.OPERATOR;
						}
						else if (nextChar == " " || nextChar == null || nextChar == "(" || nextChar == ")"){
							state = State.OPERATOR;
							store = Insert(state,store);
						}
						else if (nextChar == "E"){
							state = State.NUM;
						}
						else {
							state = State.ERROR;
							store = Insert(state,store);
						}
					}
					else if (IsNumber(prevChar) || nextChar == "+" || nextChar == "-"){
						state = State.ERROR;
						store = Insert(state,store);
					}
					else if (prevChar == "E" && (nextChar == " " || nextChar == null || nextChar == "+" ||
					                             nextChar == "-")){
						state = State.ERROR;
						store = Insert(state,store);
					}					
					else {
						if ((prevChar == "(" || prevChar == " " || prevChar == ")") && (nextChar == " " || nextChar == ")" ||
						                                                                nextChar == null || nextChar == "(")){
							state = State.OPERATOR;
							store = Insert(state,store);
						}
						else if (prevChar == "(" && IsNumber(nextChar)){
							state = State.NUM;
						}
					}
				}
				else if (currChar == "x" || currChar == ":"){ //Jika char sekarang x atau :
					store += currChar;
					if (prevChar == null){
						if (nextChar == " " || nextChar == "(" || nextChar == ")" || nextChar == null){
							state = State.OPERATOR;
							store = Insert(state,store);
						}
					}
					else {
						if ((prevChar == " " || prevChar == "(" || prevChar == ")") && (nextChar == " " || prevChar == ")" ||
						                                                                prevChar =="(" || nextChar == null)){
							state = State.OPERATOR;
							store = Insert(state,store);
						}
						else {
							state = State.ERROR;
							store = Insert(state,store);
						}
					}
				}
				else if (currChar == "("){ //Jika char sekarang adalah (
					store += currChar;
					state = State.LPAR;
					store = Insert(state,store);
				}
				else if (currChar == ")"){ //Jika char sekarang adalah )
					store += currChar;
					state = State.RPAR;
					store = Insert(state,store);
				}
				else if (currChar == " "){ //Jika char sekarang adalah spasi
					comma = false;
					Esym = false;
					store = "";
				}
				else { //Selain inputan number + - : x , E ( ) [spasi]
					store += currChar;
					state = State.ERROR;
					store = Insert(state,store);
				}
				i++; //Next iterator, cek char selanjutnya
			} while (state != State.ERROR && nextChar != null); //Berulang hingga string terakhir atau state ERROR
			#endregion
			Console.Write("Output:");			
			for (i = 0; i < lexical.Count; i++){ //Traversal list state lexical & list string stored 
				Console.Write("\n{0}\t <{1}>",stored[i],lexical[i].ToString());				
			}					
			//Checking with PDA
			#region PDA
			i=0;
			var endCheck = false;
			var parentheses = 0;
			var currState = lexical.Count > 0? lexical[0] : State.ERROR;			
			do{
				var nextI = i+1;				
				switch (currState) {
					case State.NUM:
						if (lexical.Count == 1){
							currState = State.ACCEPTED;
						}
						else if (nextI >= lexical.Count && parentheses == 0){
							currState = State.ACCEPTED;
						}
						else if (nextI >= lexical.Count && parentheses != 0){
							currState = State.ERROR;
						}
						else if (lexical[nextI] == State.OPERATOR){
							currState = State.OPERATOR;
						}
						else if (lexical[nextI] == State.LPAR){
							currState = State.LPAR;
							parentheses++;
						}
						else if (lexical[nextI] == State.RPAR){
							currState = State.RPAR;
							parentheses--;
						}
						else
							currState = State.ERROR;
						break;
					case State.OPERATOR:
						if (lexical.Count == 1 || lexical[0] == State.OPERATOR){
							currState = State.ERROR;
						}
						else if (nextI >= lexical.Count){
							currState = State.ERROR;
						}
						else if (lexical[nextI] == State.NUM){
							currState = State.NUM;
						}
						else if (lexical[nextI] == State.LPAR){
							currState = State.LPAR;
							parentheses++;
						}
						else
							currState = State.ERROR;
						break;
					case State.LPAR:
						if (i == 0){
							if (lexical[0] == State.LPAR){
								parentheses++;
							}
						}						
						if (lexical.Count == 1){
							currState = State.ERROR;
						}
						else if (nextI >= lexical.Count){
							currState = State.ERROR;
						}						
						else if (lexical[nextI] == State.NUM){
							currState = State.NUM;
						}
						else if (lexical[nextI] == State.LPAR){
							currState = State.LPAR;
							parentheses++;
						}
						else 
							currState = State.ERROR;
						break;
					case State.RPAR:						
						if (lexical.Count == 1 || lexical[0] == State.RPAR){
							currState = State.ERROR;
						}
						else if (nextI >= lexical.Count && parentheses == 0){
							currState = State.ACCEPTED;
						}
						else if (lexical[nextI] == State.OPERATOR){
							currState = State.OPERATOR;
						}
						else if (lexical[nextI] == State.RPAR){
							currState = State.RPAR;
							parentheses--;
						}
						else
							currState = State.ERROR;
						break;				
					case State.ERROR:
						Console.WriteLine("\n<INVALID>");	
						endCheck = true;
						break;
					case State.ACCEPTED:
						Console.WriteLine("\n<ACCEPTED>");
						endCheck = true;
						break;
				}				
				i++;
			} while (!endCheck);
			#endregion
			Console.WriteLine("\nEnd program . . .");
			Console.ReadKey(true);
		}
	}
}
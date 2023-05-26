package main

import (
	"database/sql"
	"fmt"
	"log"
	"os"

	"github.com/joho/godotenv"
	_ "github.com/lib/pq"
)

func main() {
	godotenv.Load()

	db_host := os.Getenv("DB_HOST")
	db_port := os.Getenv("DB_PORT")
	db_name := os.Getenv("DB_NAME")
	db_user := os.Getenv("DB_USER")
	db_password := os.Getenv("DB_PASSWORD")

	connString := fmt.Sprintf(
		"host=%s port=%s user=%s password=%s dbname=%s sslmode=disable",
		db_host,
		db_port,
		db_user,
		db_password,
		db_name,
	)

	db, err := sql.Open("postgres", connString)

	if err != nil {
		log.Fatal(err)
	}

	rows, err := db.Query("SELECT area, field, uwi FROM wells")

	if err != nil {
		log.Fatal(err)
	}

	var (
		area  string
		field string
		uwi   string
	)

	for rows.Next() {
		err := rows.Scan(&area, &field, &uwi)

		if err != nil {
			log.Fatal(err)
		}
		fmt.Println("\n", area, field, uwi)
	}

}

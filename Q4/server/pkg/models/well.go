package models

import "github.com/google/uuid"

type well struct {
	Id uuid.UUID
}

type wellResposne struct {
	Well well
}

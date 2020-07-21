module Cookbook.Server.Auth.Jwt

open System
open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens

open Cookbook.Shared.Auth

let getKey (secret:string) = SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))

let createToken audience issuer secret expiration claims =
    let credentials = SigningCredentials(getKey secret, SecurityAlgorithms.HmacSha256)
    let issuedOn = DateTime.UtcNow
    let expiresOn = issuedOn.Add(expiration)
    let jwtToken = JwtSecurityToken(issuer, audience, claims, (issuedOn |> Nullable), (expiresOn |> Nullable), credentials)
    let handler = JwtSecurityTokenHandler()
    ({ Token = handler.WriteToken(jwtToken); ExpiresOnUtc = expiresOn } : Response.Token)
[<RequireQualifiedAccess>]
module Cookbook.Libraries.Jwt

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Cryptography
open Microsoft.IdentityModel.Tokens


[<Literal>]
let DefaultRefreshKeyLength = 32

let getKey (secret:string) = SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))

let createToken audience issuer secret expiration claims =
    let credentials = SigningCredentials(getKey secret, SecurityAlgorithms.HmacSha256)
    let issuedOn = DateTime.UtcNow
    let expiresOn = issuedOn.Add(expiration)
    let jwtToken = JwtSecurityToken(issuer, audience, claims, (issuedOn |> Nullable), (expiresOn |> Nullable), credentials)
    let handler = JwtSecurityTokenHandler()
    handler.WriteToken(jwtToken),(expiresOn |> DateTimeOffset)

let createRefreshToken (size:int) expiration =
    let randomNumber = Array.create size (Byte())
    use generator = RandomNumberGenerator.Create()
    let expiresOn = DateTimeOffset.UtcNow.Add(expiration)
    generator.GetBytes(randomNumber)
    Convert.ToBase64String(randomNumber),expiresOn
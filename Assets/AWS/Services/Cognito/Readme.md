# AWS Cognito Federated Entities 

> This package is still in production

Presenting Unity + AWS Cognito Federated Entities integration.
Lightweight. Simple. Extendable. With this package you can easily integrate Authentication feature into your products that are made on Unity. Supports <b> iOS, Android, MacOS, WebGL, UWP </b>
You need some basic AWS skills in order to use that package.

## AWS Cognito Setup
In general we need to create CognitoUserPool, connect Federated Identity Providers such as: `Google, Apple, Facebook`. Create `Public App Client` without ClientSecret. In HostedUI tab `CallbackURL` should be `unitydl://mylink.com/` and `SignoutURL` can be `https://localhost:3000/`. `OAuth 2.0 grant types` should be `Implicit grant`
That is pretty much all.


Also here is few videos that should help you to understand setup process better. 
- [How to setup Cognito + Google]("https://www.youtube.com/watch?v=r1P_glQGvfo")
- [How to use AccessToken with your API](https://www.youtube.com/watch?v=o7OHogUcRmI)

## Unity Setup
First step is to setup DeepLink URL schema. Process depends on platform but overall is pretty easy. Just follow [this official documentation.]("https://docs.unity3d.com/Manual/deep-linking.html"). For simplicity purpose I keep default URL schema, but you can customize it later once you confirm that integration is totally working on your side.
### Android
Here we just need to add URL schema to AndroidManifest.xml
### iOS, MacOS, UWP
Here we just need to extend Supported URL Schemas with `unitydl`

### Testing
In `CognitoExample` scene select `CognitoTest`, expand Settings and change `ClientId` after those steps you can save scene and start testing.
Requires building App on target platform as core feature `DeepLink` doesn't work in Editor mode.

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TokenInterceptor } from './interceptors/token.interceptor';


import { AppComponent } from './app.component';
import { UserLoginComponent } from './components/user-login/user-login.component';
import { UserSignupComponent } from './components/user-signup/user-signup.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { AboutComponent } from './components/about/about.component';
import { RecommendedGamesComponent } from './components/recommended-games/recommended-games.component';
import { MainPageComponent } from './components/main-page/main-page.component';
import { SearchComponent } from  './components/search/search.component';
import { GameDisplayComponent } from './components/game-display/game-display.component';
import { IgnoreListComponent } from './components/ignore-list/ignore-list.component';


@NgModule({
  declarations: [
    AppComponent,
    UserLoginComponent,
    UserSignupComponent,
    NavbarComponent,
    AboutComponent,
    RecommendedGamesComponent,
    MainPageComponent,
    SearchComponent,
    GameDisplayComponent,
    IgnoreListComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgbModule,
    FormsModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true
}],
  bootstrap: [AppComponent],
})
export class AppModule { }
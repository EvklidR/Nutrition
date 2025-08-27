import { provideRouter, Routes } from "@angular/router";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { LoginComponent } from "../app/components/login/login.component"
import { RegisterComponent } from "../app/components/register/register.component";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async'
import { HomeComponent } from "./components/home/home.component";
import { AuthGuard } from "./guards/auth.guard";
import { SidebarComponent } from "./components/sideBar/sidebar.component";
import { ProfileSelectionComponent } from "./components/profile-selection/profile-selection.component";
import { CreateProfileComponent } from "./components/create-profile/create-profile.component";
import { ProfileGuard } from "./guards/profile.guard";
import { tokenInterceptor } from "./interceptors/token.interceptor";
import { ProfileInfoComponent } from "./components/profile-info/profile-info.component";
import { MealPlansComponent } from "./components/meal-plans/meal-plans.component";
import { CreateMealPlanComponent } from "./components/create-meal-plan/create-meal-plan.component";
import { BodyStatisticsComponent } from "./components/statistics/body-statistics/body-statistics.component";
import { FoodStatisticsComponent } from "./components/statistics/food-statistics/food-statistics.component";
import { PostsComponent } from "./components/posts/posts.component";
import { PostDetailsComponent } from "./components/post-details/post-details.component";
import { CreatePostComponent } from "./components/create-post/create-post.component";
import { FoodSwitcherComponent } from "./components/food/food-switcher/food-switcher.component";
import { ProductsComponent } from "./components/food/products/products.component";
import { DishesComponent } from "./components/food/dishes/dishes.component";
import { StatisticsSwitcherComponent } from "./components/statistics/statistics-switcher/statistics-switcher.component";
import { HistorySwitcherComponent } from "./components/history/history-switcher/history-switcher.component";

const appRoutes: Routes = [
  { path: "login", component: LoginComponent },
  { path: "register", component: RegisterComponent },
  {
    path: '',
    component: SidebarComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent, canActivate: [ProfileGuard] },
      { path: 'profile-selection', component: ProfileSelectionComponent },
      { path: 'create-profile', component: CreateProfileComponent },
      {
        path: 'food',
        component: FoodSwitcherComponent,
        canActivate: [ProfileGuard],
        children: [
          { path: 'products', component: ProductsComponent },
          { path: 'dishes', component: DishesComponent },
          { path: '', redirectTo: 'products', pathMatch: 'full' }
        ]
      },
      { path: 'profile-info', component: ProfileInfoComponent, canActivate: [ProfileGuard] },
      { path: 'meal-plans', component: MealPlansComponent, canActivate: [ProfileGuard] },
      { path: 'create-meal-plan', component: CreateMealPlanComponent },
      { path: 'meal-plan/edit/:id', component: CreateMealPlanComponent },
      {
        path: 'statistics',
        component: StatisticsSwitcherComponent,
        canActivate: [ProfileGuard],
        children: [
          { path: 'body', component: BodyStatisticsComponent },
          { path: 'food', component: FoodStatisticsComponent },
          { path: '', redirectTo: 'body', pathMatch: 'full' }
        ]
      },
      {
        path: 'history',
        component: HistorySwitcherComponent,
        canActivate: [ProfileGuard],
        children: [
          { path: 'days', component: BodyStatisticsComponent },
          { path: 'meal-plans', component: FoodStatisticsComponent },
          { path: '', redirectTo: 'days', pathMatch: 'full' }
        ]
      },
      { path: 'posts', component: PostsComponent },
      { path: 'post-details/:id', component: PostDetailsComponent },
      { path: 'create-post', component: CreatePostComponent },
      { path: 'edit-post/:id', component: CreatePostComponent }
    ]
  },];

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes),
    provideHttpClient(withInterceptors([
      tokenInterceptor
    ])),
    provideAnimationsAsync()
  ]
};

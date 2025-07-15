import { Routes } from '@angular/router';
import { Login } from './login/login';
import { CategoryList } from './components/category-list/category-list';
import { RoleList } from './components/role-list/role-list';
import { RoleCategoryList } from './components/role-category-list/role-category-list';
import { MediaTypesList } from './components/media-types-list/media-types-list';
import { UserList } from './components/user-list/user-list';
import { AccessList } from './components/access-list/access-list';
import { MainLayoutComponent } from './shared/nav/nav';
import { Dashboard } from './components/dashboard/dashboard';
import { UploadPage } from './components/upload-page/upload-page';
import { Profile } from './components/profile/profile';
import { UserFiles } from './components/user-files/user-files';
import { UserCategory } from './components/user-category/user-category';
import { AuthGaurd } from './guards/auth-guard';
import { AdminGuard } from './guards/admin-guard';
import { FileList } from './components/file-list/file-list';
import { LoginGuard } from './guards/login-guard-guard';



export const routes: Routes = [
  { path: '', component: Login , canActivate: [LoginGuard]},
  {
    path: 'home',
    component: MainLayoutComponent,
    canActivate: [AuthGaurd],
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'files', component: UserFiles },
      { path: 'upload', component: UploadPage },
      { path: 'user-category', component: UserCategory },
      { path: 'category', component: CategoryList },
      { path: 'mediaType', component: MediaTypesList },
      {
        path: 'admin',
        canActivate: [AdminGuard],
        children: [
          { path: 'role', component: RoleList },
          { path: 'roleCategory', component: RoleCategoryList },
          { path: 'users', component: UserList },
          { path: 'files', component: FileList },
          { path: 'users/:uname', component: Profile },
          { path: 'access', component: AccessList },
        ]
      },
      { path: 'profile/:uname', component: Profile },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  }
];


import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';
import { CategoryService } from './services/category.service';
import { RoleService } from './services/role.service';
import { RoleCategoryService } from './services/roleCategory.service';
import { AccessLevelService } from './services/access.service';
import { MediaTypeService } from './services/mediaType.service';
import { FileArchiveService } from './services/fileUpload.service';
import { UserService } from './services/user.service';
import { authInterceptor } from './interceptors/auth.interceptor';
import { FileVersionService } from './services/fileVersion.service';
import { EmployeeService } from './services/employee.service';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { FileSummaryService } from './services/fileSummary.service';
import { AuthGaurd } from './guards/auth-guard';
import { AdminGuard } from './guards/admin-guard';
import { LoginGuard } from './guards/login-guard-guard';


export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor , ErrorInterceptor])
    ),
    AuthService,
    NotificationService,
    CategoryService,
    RoleService,
    RoleCategoryService,
    AccessLevelService,
    MediaTypeService,
    FileArchiveService,
    FileVersionService,
    UserService,
    EmployeeService,
    FileSummaryService,
    AuthGaurd,
    AdminGuard,
    LoginGuard
  ]
};

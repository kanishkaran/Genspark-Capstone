import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../services/auth.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatNavList } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { Subscription } from 'rxjs';
import { MatFormFieldModule } from '@angular/material/form-field';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  adminOnly?: boolean;
  viewerHidden?: boolean
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    RouterLink, MatToolbarModule, MatCardModule, MatSidenavModule,
    MatIconModule, MatNavList, RouterOutlet, MatMenuModule, MatFormFieldModule
  ],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  currentUser: string | null = null;
  roleName: string | null = null;
  isAdmin = false;
  navItems: NavItem[] = [];

  private userSub?: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
   
  }
  
  ngOnInit(): void {
    this.userSub = this.authService.currUser$.subscribe(user => {
      this.currentUser = user?.username ?? null;
      this.roleName = user?.roleName;
      console.log(user)
      this.isAdmin = user.roleName == "Admin"
      console.log(this.isAdmin)
      this.setNavItems()
    });
  }

  setNavItems(): void {
    this.navItems = [
      { label: 'DashBoard', route: '/home/dashboard', icon: 'dashboard' },
      { label: 'Explorer', route: '/home/user-category', icon: 'explorer' },
      { label: 'Upload Files', route: '/home/upload', icon: 'cloud_upload', viewerHidden: true},
      { label: 'My Files', route: '/home/files', icon: 'folder' , viewerHidden: true},
      { label: 'Categories', route: '/home/category', icon: 'category' },
      { label: 'Media Types', route: '/home/mediaType', icon: 'file_copy' },
      { label: 'Users', route: '/home/admin/users', icon: 'people', adminOnly: true },
      { label: 'All Files', route: '/home/admin/files', icon: 'layers', adminOnly: true },
      { label: 'Roles', route: '/home/admin/role', icon: 'security', adminOnly: true },
      { label: 'Access Levels', route: '/home/admin/access', icon: 'vpn_key', adminOnly: true },
      { label: 'Role Permissions', route: '/home/admin/roleCategory', icon: 'admin_panel_settings', adminOnly: true }
    ];
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  profile(){
   this.router.navigate(['home', 'profile', this.currentUser]);
  }

  shouldDisplay(item: any): boolean {
    if (item.adminOnly && !this.isAdmin) return false;
    if (item.viewerHidden && this.roleName === 'Viewer') return false;
    return true;
  }

  ngOnDestroy(): void {
    this.userSub?.unsubscribe();
  }
}

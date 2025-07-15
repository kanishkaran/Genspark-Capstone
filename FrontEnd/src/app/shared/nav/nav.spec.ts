import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthService } from '../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { MainLayoutComponent } from './nav';

describe('MainLayoutComponent', () => {
  let component: MainLayoutComponent;
  let fixture: ComponentFixture<MainLayoutComponent>;

  const mockUser = { username: 'user@example.com', roleName: 'Admin' };

  const mockAuthService = {
    currUser$: of(mockUser),
    isAdmin: () => true,
    logout: jasmine.createSpy()
  };

  const mockRouter = {
    navigate: jasmine.createSpy()
  };

  const mockActivatedRoute = {
    snapshot: {
      params: {},
      queryParams: {}
    }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        Router,
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ],
      imports: [MainLayoutComponent] 
    }).compileComponents();

    fixture = TestBed.createComponent(MainLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component ', () => {
    expect(component).toBeTruthy();
  });


});

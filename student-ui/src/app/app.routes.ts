import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login.component';
import { JournalComponent } from './pages/journal/journal.component';
import { AuthGuard } from './core/auth.guard';
import { AttendanceComponent } from './pages/attendance/attendance.component';
import { AssignmentsComponent } from './pages/assignments/assignments.component';
import { GradesComponent } from './pages/grades/grades.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'journal', component: JournalComponent, canActivate: [AuthGuard] },
  { path: 'attendance', component: AttendanceComponent, canActivate: [AuthGuard] },
  { path: 'assignments', component: AssignmentsComponent, canActivate: [AuthGuard] },
  { path: 'grades', component: GradesComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: 'journal', pathMatch: 'full' },
  { path: '**', redirectTo: 'journal' }
];

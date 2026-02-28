import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  StructureService,
  AcademicYear,
  Semester,
  StudentGroup,
  Discipline,
  Professor
} from '../../services/structure.service';
import { DisciplineOfferingService, DisciplineOffering } from '../../services/discipline-offering.service';
import { AuthService } from '../../services/auth.service';

type StructureSection = 'plans' | 'offerings' | 'groups' | 'disciplines' | 'calendar';

@Component({
  selector: 'app-structure',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './structure.component.html',
  styleUrls: ['./structure.component.css']
})
export class StructureComponent implements OnInit {
  role: string | null = null;
  activeSection: StructureSection = 'plans';

  years: AcademicYear[] = [];
  semesters: Semester[] = [];
  groups: StudentGroup[] = [];
  disciplines: Discipline[] = [];
  offerings: DisciplineOffering[] = [];
  professors: Professor[] = [];

  yearStart = new Date().getFullYear();
  yearEnd = new Date().getFullYear() + 1;

  semesterNumber = 1;
  semesterAcademicYearId = '';

  groupName = '';
  groupStudyYear = 1;
  groupAcademicYearId = '';

  disciplineName = '';
  disciplineHours = 72;
  disciplineControlType = 0;

  offeringDisciplineId = '';
  offeringSemesterId = '';
  offeringGroupId = '';
  offeringProfessorId = '';

  selectedPlanGroupId = '';
  planAcademicYearFilterId = '';
  planSemesterFilter = '';

  showYearForm = false;
  showSemesterForm = false;
  showGroupForm = false;
  showDisciplineForm = false;
  showOfferingForm = false;

  deletingKey = '';
  error = '';

  confirmOpen = false;
  confirmTitle = '';
  confirmText = '';
  private confirmAction: (() => void) | null = null;

  constructor(
    private structureService: StructureService,
    private offeringService: DisciplineOfferingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadAll();
  }

  get isAdmin(): boolean {
    return this.role === 'Admin';
  }

  setSection(section: StructureSection): void {
    this.activeSection = section;
  }

  loadAll(): void {
    this.error = '';

    this.structureService.getAcademicYears().subscribe({
      next: (data) => (this.years = data),
      error: () => (this.error = 'Не удалось загрузить учебные годы')
    });

    this.structureService.getSemesters().subscribe({
      next: (data) => (this.semesters = data),
      error: () => (this.error = 'Не удалось загрузить семестры')
    });

    this.structureService.getGroups().subscribe({
      next: (data) => {
        this.groups = data;
        if (!this.selectedPlanGroupId && data.length > 0) {
          this.selectedPlanGroupId = data[0].id;
        }
      },
      error: () => (this.error = 'Не удалось загрузить группы')
    });

    this.structureService.getDisciplines().subscribe({
      next: (data) => (this.disciplines = data),
      error: () => (this.error = 'Не удалось загрузить дисциплины')
    });

    this.structureService.getProfessors().subscribe({
      next: (data) => (this.professors = data),
      error: () => (this.error = 'Не удалось загрузить преподавателей')
    });

    this.offeringService.getAll().subscribe({
      next: (data) => (this.offerings = data),
      error: () => (this.error = 'Не удалось загрузить назначения дисциплин')
    });
  }

  isDeleting(key: string): boolean {
    return this.deletingKey === key;
  }

  openConfirm(title: string, text: string, action: () => void): void {
    this.confirmTitle = title;
    this.confirmText = text;
    this.confirmAction = action;
    this.confirmOpen = true;
  }

  closeConfirm(): void {
    this.confirmOpen = false;
    this.confirmTitle = '';
    this.confirmText = '';
    this.confirmAction = null;
  }

  proceedConfirm(): void {
    const action = this.confirmAction;
    this.closeConfirm();
    action?.();
  }

  addYear(): void {
    if (!this.isAdmin) return;

    this.structureService
      .createAcademicYear({ startYear: this.yearStart, endYear: this.yearEnd })
      .subscribe({
        next: () => {
          this.yearStart = new Date().getFullYear();
          this.yearEnd = new Date().getFullYear() + 1;
          this.showYearForm = false;
          this.loadAll();
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось создать учебный год')
      });
  }

  addSemester(): void {
    if (!this.isAdmin || !this.semesterAcademicYearId) return;

    this.structureService
      .createSemester({ number: this.semesterNumber, academicYearId: this.semesterAcademicYearId })
      .subscribe({
        next: () => {
          this.semesterNumber = 1;
          this.semesterAcademicYearId = '';
          this.showSemesterForm = false;
          this.loadAll();
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось создать семестр')
      });
  }

  addGroup(): void {
    if (!this.isAdmin || !this.groupName.trim() || !this.groupAcademicYearId) return;

    this.structureService
      .createGroup({
        name: this.groupName.trim(),
        studyYear: this.groupStudyYear,
        academicYearId: this.groupAcademicYearId
      })
      .subscribe({
        next: () => {
          this.groupName = '';
          this.groupStudyYear = 1;
          this.groupAcademicYearId = '';
          this.showGroupForm = false;
          this.loadAll();
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось создать группу')
      });
  }

  addDiscipline(): void {
    if (!this.isAdmin || !this.disciplineName.trim()) return;

    this.structureService
      .createDiscipline({
        name: this.disciplineName.trim(),
        hours: this.disciplineHours,
        controlType: this.disciplineControlType
      })
      .subscribe({
        next: () => {
          this.disciplineName = '';
          this.disciplineHours = 72;
          this.disciplineControlType = 0;
          this.showDisciplineForm = false;
          this.loadAll();
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось создать дисциплину')
      });
  }

  addOffering(): void {
    if (!this.isAdmin) return;

    if (!this.offeringDisciplineId || !this.offeringSemesterId || !this.offeringGroupId || !this.offeringProfessorId) {
      return;
    }

    this.offeringService
      .create({
        disciplineId: this.offeringDisciplineId,
        semesterId: this.offeringSemesterId,
        studentGroupId: this.offeringGroupId,
        proffessorId: this.offeringProfessorId
      })
      .subscribe({
        next: () => {
          this.offeringDisciplineId = '';
          this.offeringSemesterId = '';
          this.offeringGroupId = '';
          this.offeringProfessorId = '';
          this.showOfferingForm = false;
          this.loadAll();
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось назначить дисциплину')
      });
  }

  deleteAcademicYear(id: string, label: string): void {
    this.openConfirm('Удаление учебного года', `Удалить учебный год ${label}?`, () => {
      this.deletingKey = `year-${id}`;
      this.structureService.deleteAcademicYear(id).subscribe({
        next: () => {
          this.deletingKey = '';
          this.loadAll();
        },
        error: (e) => {
          this.deletingKey = '';
          this.error = e?.error?.message ?? 'Не удалось удалить учебный год. Возможно, есть связанные записи.';
        }
      });
    });
  }

  deleteSemester(id: string): void {
    this.openConfirm('Удаление семестра', 'Удалить семестр?', () => {
      this.deletingKey = `semester-${id}`;
      this.structureService.deleteSemester(id).subscribe({
        next: () => {
          this.deletingKey = '';
          this.loadAll();
        },
        error: (e) => {
          this.deletingKey = '';
          this.error = e?.error?.message ?? 'Не удалось удалить семестр. Возможно, есть связанные записи.';
        }
      });
    });
  }

  deleteGroup(id: string, name: string): void {
    this.openConfirm('Удаление группы', `Удалить группу ${name}?`, () => {
      this.deletingKey = `group-${id}`;
      this.structureService.deleteGroup(id).subscribe({
        next: () => {
          this.deletingKey = '';
          this.loadAll();
        },
        error: (e) => {
          this.deletingKey = '';
          this.error = e?.error?.message ?? 'Не удалось удалить группу. Возможно, есть связанные записи.';
        }
      });
    });
  }

  deleteDiscipline(id: string, name: string): void {
    this.openConfirm('Удаление дисциплины', `Удалить дисциплину ${name}?`, () => {
      this.deletingKey = `discipline-${id}`;
      this.structureService.deleteDiscipline(id).subscribe({
        next: () => {
          this.deletingKey = '';
          this.loadAll();
        },
        error: (e) => {
          this.deletingKey = '';
          this.error = e?.error?.message ?? 'Не удалось удалить дисциплину. Возможно, есть связанные записи.';
        }
      });
    });
  }

  deleteOffering(id: string): void {
    this.openConfirm('Удаление назначения', 'Удалить назначение дисциплины?', () => {
      this.deletingKey = `offering-${id}`;
      this.offeringService.delete(id).subscribe({
        next: () => {
          this.deletingKey = '';
          this.loadAll();
        },
        error: (e) => {
          this.deletingKey = '';
          this.error = e?.error?.message ?? 'Не удалось удалить назначение дисциплины. Возможно, есть ведомости или оценки.';
        }
      });
    });
  }

  controlTypeLabel(type: number): string {
    if (type === 1) return 'Зачет';
    if (type === 2) return 'Дифференцированный зачет';
    return 'Экзамен';
  }

  academicYearLabel(academicYearId: string): string {
    const year = this.years.find((y) => y.id === academicYearId);
    return year ? `${year.startYear}/${year.endYear}` : '—';
  }

  filteredPlanRows(): DisciplineOffering[] {
    return this.sortedOfferings().filter((o) => {
      if (this.selectedPlanGroupId && o.studentGroupId !== this.selectedPlanGroupId) {
        return false;
      }

      if (this.planAcademicYearFilterId && o.semester?.academicYear?.id !== this.planAcademicYearFilterId) {
        return false;
      }

      if (this.planSemesterFilter && String(o.semester?.number ?? '') !== this.planSemesterFilter) {
        return false;
      }

      return true;
    });
  }

  sortedYears(): AcademicYear[] {
    return [...this.years].sort((a, b) => a.startYear - b.startYear);
  }

  sortedSemesters(): Semester[] {
    return [...this.semesters].sort((a, b) => {
      const aYear = this.years.find((y) => y.id === a.academicYearId)?.startYear ?? 0;
      const bYear = this.years.find((y) => y.id === b.academicYearId)?.startYear ?? 0;
      if (aYear !== bYear) return aYear - bYear;
      return a.number - b.number;
    });
  }

  sortedGroups(): StudentGroup[] {
    return [...this.groups].sort((a, b) => a.name.localeCompare(b.name));
  }

  sortedDisciplines(): Discipline[] {
    return [...this.disciplines].sort((a, b) => a.name.localeCompare(b.name));
  }

  sortedOfferings(): DisciplineOffering[] {
    return [...this.offerings].sort((a, b) => {
      const gA = a.studentGroup?.name ?? '';
      const gB = b.studentGroup?.name ?? '';
      if (gA !== gB) return gA.localeCompare(gB);

      const ayA = a.semester?.academicYear?.startYear ?? 0;
      const ayB = b.semester?.academicYear?.startYear ?? 0;
      if (ayA !== ayB) return ayA - ayB;

      const semA = a.semester?.number ?? 0;
      const semB = b.semester?.number ?? 0;
      if (semA !== semB) return semA - semB;

      const dA = a.discipline?.name ?? '';
      const dB = b.discipline?.name ?? '';
      return dA.localeCompare(dB);
    });
  }
}

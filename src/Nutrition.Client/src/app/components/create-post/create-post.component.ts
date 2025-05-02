import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { CommonModule } from '@angular/common';
import { PostService } from '../../services/post-service/post.service';
import { CreatePostModel } from '../../models/post-service/Requests/create-post.model';
import { PostModel } from '../../models/post-service/Responses/post.model';
import { Router, ActivatedRoute } from '@angular/router';
import { UpdatePostModel } from '../../models/post-service/Requests/update-post.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MarkdownModule
  ],
  providers: [provideMarkdown()],
  templateUrl: './create-post.component.html',
  styleUrls: ['./create-post.component.css']
})
export class CreatePostComponent implements OnInit {
  titleControl = new FormControl('');
  contentControl = new FormControl('');
  previewMode = false;

  postToEdit: PostModel | null = null;

  @ViewChild('contentArea') contentArea!: ElementRef<HTMLTextAreaElement>;

  constructor(
    private postService: PostService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.postService.getPost(postId).subscribe({
        next: (post) => {
          this.postToEdit = post;
          this.titleControl.setValue(post.title);
          this.contentControl.setValue(post.text);
        },
        error: (err) => {
          console.error('Ошибка загрузки поста:', err);
          this.router.navigate(['/posts']);
        }
      });
    }
  }

  togglePreview(): void {
    this.previewMode = !this.previewMode;
  }

  formatText(type: 'bold' | 'italic' | 'heading1' | 'heading2' | 'heading3' | 'strikethrough'): void {
    const textarea = this.contentArea.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = this.contentControl.value?.substring(start, end) || '';

    let formatted = '';
    switch (type) {
      case 'bold': formatted = `**${selectedText}**`; break;
      case 'italic': formatted = `*${selectedText}*`; break;
      case 'strikethrough': formatted = `~~${selectedText}~~`; break;
      case 'heading1': formatted = `# ${selectedText}`; break;
      case 'heading2': formatted = `## ${selectedText}`; break;
      case 'heading3': formatted = `### ${selectedText}`; break;
    }

    const before = this.contentControl.value?.substring(0, start) || '';
    const after = this.contentControl.value?.substring(end) || '';
    const newText = before + formatted + after;

    this.contentControl.setValue(newText);

    setTimeout(() => {
      const pos = start + formatted.length;
      textarea.setSelectionRange(pos, pos);
      textarea.focus();
    });
  }

  submitPost(): void {
    const title = this.titleControl.value?.trim();
    const content = this.contentControl.value?.trim();

    if (!content) {

      this.snackBar.open('Содержание не может быть пустым!', 'Close', {
        duration: 3000
      });
      return
    };

    if (this.postToEdit) {
      const updateData: UpdatePostModel = {
        id: this.postToEdit.id,
        title: title ?? '',
        text: content,
        keyWords: this.postToEdit.keyWords ?? ['angular', 'markdown'],
        newFiles: []
      };

      this.postService.updatePost(updateData).subscribe({
        next: () => this.router.navigate(['/posts']),
        error: (err) => console.error('Ошибка при обновлении поста:', err)
      });

    } else {
      const postData: CreatePostModel = {
        title: title ?? '',
        text: content,
        keyWords: ['angular', 'markdown'],
        ownerEmail: null,
        ownerId: null,
        files: []
      };

      this.postService.createPost(postData).subscribe({
        next: () => {
          this.titleControl.setValue('');
          this.contentControl.setValue('');
          this.previewMode = false;
          this.router.navigate(['/posts']);
        },
        error: (err) => console.error('Ошибка при создании поста:', err)
      });
    }
  }
}

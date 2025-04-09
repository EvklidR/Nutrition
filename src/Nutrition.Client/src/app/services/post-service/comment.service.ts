import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateCommentModel } from '../../models/post-service/Requests/create-comment.model';
import { UpdateCommentModel } from '../../models/post-service/Requests/update-comment.model';
import { CommentModel } from '../../models/post-service/Responses/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly baseUrl: string = 'https://localhost/post_service/comments';

  constructor(private http: HttpClient) { }

  getComments(postId: string, page: number | null, size: number | null): Observable<CommentModel[]> {
    return this.http.get<CommentModel[]>(`${this.baseUrl}/${postId}?page=${page}&size=${size}`);
  }

  addComment(commentData: CreateCommentModel): Observable<CommentModel> {
    return this.http.post<CommentModel>(this.baseUrl, commentData);
  }

  deleteComment(commentId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${commentId}`);
  }

  updateComment(commentData: UpdateCommentModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, commentData);
  }

  likeComment(commentId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${commentId}/like`, {});
  }
}

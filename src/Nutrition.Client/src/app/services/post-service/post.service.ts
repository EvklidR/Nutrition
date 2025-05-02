import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreatePostModel } from '../../models/post-service/Requests/create-post.model'
import { UpdatePostModel } from '../../models/post-service/Requests/update-post.model'
import { PostModel } from '../../models/post-service/Responses/post.model'
import { PostsModel } from '../../models/post-service/Responses/posts.model'

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private readonly baseUrl: string = 'https://localhost/post_service/posts';

  constructor(private http: HttpClient) { }

  getPosts(words?: string[], page: number = 1, size: number = 10): Observable<PostsModel> {
    let params = `?page=${page}&size=${size}`;

    if (words?.length) {
      params += `&words=${words.join(',')}`;
    }

    return this.http.get<PostsModel>(`${this.baseUrl}${params}`);
  }

  getPost(postId: string): Observable<PostModel> {
    return this.http.get<PostModel>(`${this.baseUrl}/${postId}`);
  }

  getUserPosts(page: number = 1, size: number = 10): Observable<PostsModel> {
    return this.http.get<PostsModel>(`${this.baseUrl}/by_user?page=${page}&size=${size}`);
  }

  createPost(postData: CreatePostModel): Observable<PostModel> {
    const formData = new FormData();
    formData.append('title', postData.title);
    formData.append('text', postData.text);

    postData.keyWords.forEach((word) => {
      formData.append('keyWords', word);
    });

    postData.files.forEach((file) => {
      formData.append('files', file);
    });

    return this.http.post<PostModel>(`${this.baseUrl}`, formData);
  }


  deletePost(postId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${postId}`);
  }

  updatePost(postData: UpdatePostModel): Observable<void> {
    const formData = new FormData();
    formData.append('id', postData.id);
    formData.append('title', postData.title);
    formData.append('text', postData.text);

    postData.keyWords.forEach((word) => {
      formData.append('keyWords', word);
    });

    postData.newFiles.forEach((file) => {
      formData.append('newFiles', file);
    });

    return this.http.put<void>(`${this.baseUrl}`, formData);
  }

  likePost(postId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${postId}/like`, {});
  }

  getPostImage(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${fileName}`, { responseType: 'blob' });
  }
}
